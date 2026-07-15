// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Text;
using System.Text.RegularExpressions;

namespace ClangSharp;

// A precompiled shell-style glob used by the name-matching options. Supports `*` (matches any run
// of characters, including qualification separators) and `?` (matches a single character); every
// other character is a literal. Matching is case-sensitive and, like QualifiedNameComparer, treats
// the `::` and `.` qualification separators as equivalent, so `NS::*` and `NS.*` behave the same.
internal sealed class GlobPattern
{
    // Matches either qualification separator so a literal separator in a pattern is equivalent
    // whether the candidate name uses `::` (qualified names) or `.` (remapped names).
    private const string SeparatorPattern = "(?:::|\\.)";

    private readonly Regex _regex;

    private GlobPattern(string pattern, Regex regex, int literalLength)
    {
        Pattern = pattern;
        _regex = regex;
        LiteralLength = literalLength;
    }

    // The original pattern text, as provided on the command line or in the config.
    public string Pattern { get; }

    // The number of literal (non-`*`/`?`) characters, used to rank competing globs so the most
    // specific pattern wins.
    public int LiteralLength { get; }

    // A pattern is a glob when it contains a `*` or `?` and is not the bare `*` catch-all, which
    // keeps its own dedicated handling in the callers.
    public static bool IsGlob(ReadOnlySpan<char> pattern)
        => (pattern.Length != 1 || pattern[0] != '*') && (pattern.IndexOf('*') != -1 || pattern.IndexOf('?') != -1);

    public static GlobPattern Compile(string pattern)
    {
        var builder = new StringBuilder(pattern.Length + 16);
        _ = builder.Append('^');

        var literalLength = 0;

        for (var i = 0; i < pattern.Length; i++)
        {
            var c = pattern[i];

            switch (c)
            {
                case '*':
                {
                    _ = builder.Append(".*");
                    break;
                }

                case '?':
                {
                    _ = builder.Append('.');
                    break;
                }

                case '.':
                {
                    _ = builder.Append(SeparatorPattern);
                    literalLength++;
                    break;
                }

                case ':' when (i + 1 < pattern.Length) && (pattern[i + 1] == ':'):
                {
                    _ = builder.Append(SeparatorPattern);
                    literalLength++;
                    i++;
                    break;
                }

                default:
                {
                    _ = builder.Append(Regex.Escape(c.ToString()));
                    literalLength++;
                    break;
                }
            }
        }

        _ = builder.Append('$');

        var regex = new Regex(builder.ToString(), RegexOptions.CultureInvariant | RegexOptions.Compiled);
        return new GlobPattern(pattern, regex, literalLength);
    }

    public bool IsMatch(ReadOnlySpan<char> name) => _regex.IsMatch(name);
}
