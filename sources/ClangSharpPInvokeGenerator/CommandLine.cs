// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ClangSharp;

internal enum CommandLineOptionKind
{
    Flag,
    SingleValue,
    MultipleValue,
}

internal sealed class CommandLineOption
{
    private readonly List<string> _values;

    public CommandLineOption(string[] aliases, string description, CommandLineOptionKind kind, string? valueName = null, string? defaultValue = null, string[]? allowedValues = null)
    {
        Aliases = aliases;
        Description = description;
        Kind = kind;
        ValueName = valueName ?? aliases[0].TrimStart('-');
        DefaultValue = defaultValue;
        AllowedValues = allowedValues;
        _values = [];
    }

    public string[] Aliases { get; }

    public string[]? AllowedValues { get; }

    // The canonical name used in diagnostics; the first alias is always the long form.
    public string CanonicalName => Aliases[0];

    public string? DefaultValue { get; }

    public string Description { get; }

    public bool IsPresent { get; set; }

    public CommandLineOptionKind Kind { get; }

    public string ValueName { get; }

    // The effective value for a single-value option: the last one specified or the default.
    public string SingleValue => (_values.Count != 0) ? _values[^1] : (DefaultValue ?? "");

    public void AddValue(string value) => _values.Add(value);

    public string[] GetValues() => [.. _values];
}

internal readonly struct HelpRow(string term, string description)
{
    public string Term { get; } = term;

    public string Description { get; } = description;
}

internal sealed class CommandLineParser
{
    private const int MaxResponseFileDepth = 32;

    private readonly IReadOnlyList<CommandLineOption> _options;
    private readonly Dictionary<string, CommandLineOption> _optionsByAlias;
    private readonly List<string> _errors;

    public CommandLineParser(IReadOnlyList<CommandLineOption> options)
    {
        _options = options;
        _optionsByAlias = new Dictionary<string, CommandLineOption>(StringComparer.Ordinal);
        _errors = [];

        foreach (var option in options)
        {
            foreach (var alias in option.Aliases)
            {
                _optionsByAlias.Add(alias, option);
            }
        }
    }

    public IReadOnlyList<string> Errors => _errors;

    public void Parse(IReadOnlyList<string> args)
    {
        var tokens = new List<string>();

        foreach (var arg in args)
        {
            ExpandToken(arg, tokens, depth: 0);
        }

        var index = 0;

        while (index < tokens.Count)
        {
            var token = tokens[index];

            if (!TryMatchOption(token, out var option, out var inlineValue))
            {
                _errors.Add($"Error: Unrecognized command or argument '{token}'.");
                index++;
                continue;
            }

            index++;
            option.IsPresent = true;

            if (option.Kind == CommandLineOptionKind.Flag)
            {
                if (inlineValue is not null)
                {
                    _errors.Add($"Error: Option '{option.CanonicalName}' does not accept an argument.");
                }
                continue;
            }

            if (inlineValue is not null)
            {
                AddValue(option, inlineValue);
                continue;
            }

            var count = 0;

            while ((index < tokens.Count) && !TryMatchOption(tokens[index], out _, out _))
            {
                AddValue(option, tokens[index]);
                index++;
                count++;

                if (option.Kind == CommandLineOptionKind.SingleValue)
                {
                    break;
                }
            }

            if (count == 0)
            {
                _errors.Add($"Error: Required argument missing for option: '{option.CanonicalName}'.");
            }
        }
    }

    private void AddValue(CommandLineOption option, string value)
    {
        if ((option.AllowedValues is not null) && !option.AllowedValues.Contains(value, StringComparer.Ordinal))
        {
            _errors.Add($"Error: Argument '{value}' not recognized for option '{option.CanonicalName}'. Must be one of: {string.Join(", ", option.AllowedValues)}.");
            return;
        }

        option.AddValue(value);
    }

    private bool TryMatchOption(string token, out CommandLineOption option, out string? inlineValue)
    {
        inlineValue = null;

        if (_optionsByAlias.TryGetValue(token, out option!))
        {
            return true;
        }

        // Support the `--alias=value` (or `-alias=value`) inline form, but only when the text
        // before the first '=' is itself a known alias. This keeps bare `name=value` tokens
        // (such as a `--remap` value) intact even when the value contains '=' or spaces.

        if ((token.Length != 0) && (token[0] == '-'))
        {
            var equalsIndex = token.IndexOf('=', StringComparison.Ordinal);

            if (equalsIndex > 0)
            {
                var alias = token[..equalsIndex];

                if (_optionsByAlias.TryGetValue(alias, out option!))
                {
                    inlineValue = token[(equalsIndex + 1)..];
                    return true;
                }
            }
        }

        option = null!;
        return false;
    }

    private void ExpandToken(string token, List<string> result, int depth)
    {
        if ((token.Length == 0) || (token[0] != '@'))
        {
            result.Add(token);
            return;
        }

        var responseFilePath = token[1..];

        if (depth > MaxResponseFileDepth)
        {
            _errors.Add($"Error: Response file recursion limit exceeded at '{responseFilePath}'.");
            return;
        }

        if (!File.Exists(responseFilePath))
        {
            _errors.Add($"Error: Response file not found '{responseFilePath}'.");
            return;
        }

        string[] lines;

        try
        {
            lines = File.ReadAllLines(responseFilePath);
        }
        catch (IOException ex)
        {
            _errors.Add($"Error: {ex.Message}");
            return;
        }

        // Each non-empty, non-comment line is treated as a single token; the contents are
        // never split on whitespace so a value containing spaces is preserved as-is.

        foreach (var line in lines)
        {
            var trimmed = line.Trim();

            if ((trimmed.Length == 0) || (trimmed[0] == '#'))
            {
                continue;
            }

            ExpandToken(trimmed, result, depth + 1);
        }
    }

    public void WriteHelp(TextWriter writer, string name, string description, string epilogTitle, string epilog, string moreInfoTitle, string moreInfo)
    {
        writer.WriteLine(name);
        writer.WriteLine($"  {description}");
        writer.WriteLine();
        writer.WriteLine("Usage:");
        writer.WriteLine($"  {name} [options]");
        writer.WriteLine();
        writer.WriteLine("Options:");

        var terms = new string[_options.Count];
        var maxTermLength = 0;

        for (var i = 0; i < _options.Count; i++)
        {
            var option = _options[i];
            var term = string.Join(", ", GetDisplayAliases(option));

            if (option.Kind != CommandLineOptionKind.Flag)
            {
                term += $" <{option.ValueName}>";
            }

            terms[i] = term;
            maxTermLength = Math.Max(maxTermLength, term.Length);
        }

        for (var i = 0; i < _options.Count; i++)
        {
            var option = _options[i];
            writer.WriteLine($"  {terms[i].PadRight(maxTermLength + 2)}{option.Description}{GetDefaultSuffix(option)}");
        }

        writer.WriteLine();
        writer.WriteLine(epilogTitle);
        writer.WriteLine(epilog);
        writer.WriteLine();
        writer.WriteLine(moreInfoTitle);
        writer.WriteLine(moreInfo);
    }

    public static void WriteOptionHelp(TextWriter writer, CommandLineOption option, IReadOnlyList<HelpRow> rows)
    {
        writer.WriteLine($"{string.Join(", ", option.Aliases)}\t{option.Description}");
        writer.WriteLine();
        WriteTwoColumn(writer, rows);
    }

    public static void WriteTwoColumn(TextWriter writer, IReadOnlyList<HelpRow> rows)
    {
        writer.WriteLine("Options:");

        var maxTermLength = 0;

        foreach (var row in rows)
        {
            if (row.Description.Length != 0)
            {
                maxTermLength = Math.Max(maxTermLength, row.Term.Length);
            }
        }

        foreach (var row in rows)
        {
            if (row.Description.Length == 0)
            {
                writer.WriteLine((row.Term.Length == 0) ? "" : $"  {row.Term}");
            }
            else
            {
                writer.WriteLine($"  {row.Term.PadRight(maxTermLength + 2)}{row.Description}");
            }
        }

        writer.WriteLine();
    }

    private static IEnumerable<string> GetDisplayAliases(CommandLineOption option)
    {
        // Match the historical help ordering: short (`-x`) aliases first, then long (`--xx`).
        return option.Aliases.Where(static alias => !alias.StartsWith("--", StringComparison.Ordinal))
                    .Concat(option.Aliases.Where(static alias => alias.StartsWith("--", StringComparison.Ordinal)));
    }

    private static string GetDefaultSuffix(CommandLineOption option)
    {
        if (option.Kind == CommandLineOptionKind.Flag)
        {
            return "";
        }

        return string.IsNullOrEmpty(option.DefaultValue) ? " []" : $" [default: {option.DefaultValue}]";
    }
}
