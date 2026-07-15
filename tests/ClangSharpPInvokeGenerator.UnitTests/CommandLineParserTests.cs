// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.IO;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

public sealed class CommandLineParserTests
{
    private static CommandLineOption Multi(params string[] aliases)
        => new(aliases, "", CommandLineOptionKind.MultipleValue);

    private static CommandLineOption Single(params string[] aliases)
        => new(aliases, "", CommandLineOptionKind.SingleValue);

    private static CommandLineOption Flag(params string[] aliases)
        => new(aliases, "", CommandLineOptionKind.Flag);

    [Test]
    public void DeprecatedAliasStillMatchesButEmitsAWarning()
    {
        var methodClass = new CommandLineOption(["--method-class-name", "-m"], "", CommandLineOptionKind.SingleValue, deprecatedAliases: ["--methodClassName"]);
        var parser = new CommandLineParser([methodClass]);

        parser.Parse(["--methodClassName", "Methods"]);

        Assert.That(parser.Errors, Is.Empty);
        Assert.That(methodClass.SingleValue, Is.EqualTo("Methods"));
        Assert.That(parser.Warnings, Has.Count.EqualTo(1));
        Assert.That(parser.Warnings[0], Does.Contain("--methodClassName"));
        Assert.That(parser.Warnings[0], Does.Contain("--method-class-name"));
    }

    [Test]
    public void CanonicalAliasEmitsNoDeprecationWarning()
    {
        var methodClass = new CommandLineOption(["--method-class-name", "-m"], "", CommandLineOptionKind.SingleValue, deprecatedAliases: ["--methodClassName"]);
        var parser = new CommandLineParser([methodClass]);

        parser.Parse(["--method-class-name", "Methods"]);

        Assert.That(parser.Errors, Is.Empty);
        Assert.That(parser.Warnings, Is.Empty);
    }

    // Regression test for dotnet/clangsharp#554: a `name=value` token whose value contains
    // spaces must be preserved as a single value rather than split on whitespace.
    [Test]
    public void SpacedValueIsPreservedAsSingleValue()
    {
        var remap = Multi("--remap", "-r");
        var parser = new CommandLineParser([remap]);

        parser.Parse(["--remap", "__arglist=@params string[] args"]);

        Assert.That(parser.Errors, Is.Empty);
        Assert.That(remap.GetValues(), Is.EqualTo(new[] { "__arglist=@params string[] args" }));
    }

    [Test]
    public void MultiplePairsAcrossSeparateTokensAreDistinctValues()
    {
        var remap = Multi("--remap", "-r");
        var parser = new CommandLineParser([remap]);

        parser.Parse(["--remap", "A=B", "C=D"]);

        Assert.That(parser.Errors, Is.Empty);
        Assert.That(remap.GetValues(), Is.EqualTo(new[] { "A=B", "C=D" }));
    }

    [Test]
    public void InlineEqualsFormIsSplitOnlyOnTheAliasBoundary()
    {
        var remap = Multi("--remap", "-r");
        var parser = new CommandLineParser([remap]);

        parser.Parse(["--remap=Foo=System.String bar"]);

        Assert.That(parser.Errors, Is.Empty);
        Assert.That(remap.GetValues(), Is.EqualTo(new[] { "Foo=System.String bar" }));
    }

    [Test]
    public void BareNameValueTokenIsTreatedAsValueNotOption()
    {
        var remap = Multi("--remap", "-r");
        var parser = new CommandLineParser([remap]);

        parser.Parse(["--remap", "-Foo=Bar"]);

        Assert.That(parser.Errors, Is.Empty);
        Assert.That(remap.GetValues(), Is.EqualTo(new[] { "-Foo=Bar" }));
    }

    // A value that happens to start with a known short alias prefix (e.g. `-m64` after
    // `--additional`, where `-m` is a distinct option) must be kept as a value.
    [Test]
    public void ValueStartingWithAnotherOptionPrefixIsNotMatchedAsThatOption()
    {
        var additional = Multi("--additional", "-a");
        var methodClass = Single("--methodClassName", "-m");
        var parser = new CommandLineParser([additional, methodClass]);

        parser.Parse(["--additional", "-m64"]);

        Assert.That(parser.Errors, Is.Empty);
        Assert.That(additional.GetValues(), Is.EqualTo(new[] { "-m64" }));
        Assert.That(methodClass.IsPresent, Is.False);
    }

    [Test]
    public void SingleValueOptionConsumesExactlyOneToken()
    {
        var output = Single("--output", "-o");
        var remap = Multi("--remap", "-r");
        var parser = new CommandLineParser([output, remap]);

        parser.Parse(["--output", "out.cs", "--remap", "A=B"]);

        Assert.That(parser.Errors, Is.Empty);
        Assert.That(output.SingleValue, Is.EqualTo("out.cs"));
        Assert.That(remap.GetValues(), Is.EqualTo(new[] { "A=B" }));
    }

    [Test]
    public void FlagOptionTakesNoArgument()
    {
        var version = Flag("--version");
        var remap = Multi("--remap", "-r");
        var parser = new CommandLineParser([version, remap]);

        parser.Parse(["--version", "--remap", "A=B"]);

        Assert.That(parser.Errors, Is.Empty);
        Assert.That(version.IsPresent, Is.True);
        Assert.That(remap.GetValues(), Is.EqualTo(new[] { "A=B" }));
    }

    [Test]
    public void UnrecognizedOptionProducesAnError()
    {
        var remap = Multi("--remap", "-r");
        var parser = new CommandLineParser([remap]);

        parser.Parse(["--unknown"]);

        Assert.That(parser.Errors, Has.Count.EqualTo(1));
        Assert.That(parser.Errors[0], Does.Contain("--unknown"));
    }

    [Test]
    public void AllowedValuesRejectsUnexpectedValue()
    {
        var language = new CommandLineOption(["--language", "-x"], "", CommandLineOptionKind.SingleValue, allowedValues: ["c", "c++"]);
        var parser = new CommandLineParser([language]);

        parser.Parse(["--language", "rust"]);

        Assert.That(parser.Errors, Has.Count.EqualTo(1));
        Assert.That(parser.Errors[0], Does.Contain("rust"));
    }

    // A response file must yield one token per non-empty, non-comment line so that a value
    // containing spaces survives intact (the core of the #554 fix).
    [Test]
    public void ResponseFileTreatsEachLineAsASingleToken()
    {
        var remap = Multi("--remap", "-r");
        var parser = new CommandLineParser([remap]);

        var responseFile = Path.GetTempFileName();

        try
        {
            File.WriteAllLines(responseFile, [
                "# a comment",
                "--remap",
                "__arglist=@params string[] args",
                "",
                "Foo=System.String bar",
            ]);

            parser.Parse([$"@{responseFile}"]);

            Assert.That(parser.Errors, Is.Empty);
            Assert.That(remap.GetValues(), Is.EqualTo(new[] { "__arglist=@params string[] args", "Foo=System.String bar" }));
        }
        finally
        {
            File.Delete(responseFile);
        }
    }

    [Test]
    public void MissingResponseFileProducesAnError()
    {
        var remap = Multi("--remap", "-r");
        var parser = new CommandLineParser([remap]);

        parser.Parse(["@does-not-exist.rsp"]);

        Assert.That(parser.Errors, Has.Count.EqualTo(1));
        Assert.That(parser.Errors[0], Does.Contain("Response file not found"));
    }
}
