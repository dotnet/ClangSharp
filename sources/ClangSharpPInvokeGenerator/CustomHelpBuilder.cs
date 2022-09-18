// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Portions of this code are ported from https://github.com/dotnet/command-line-api
// The original source is Copyright © .NET Foundation and Contributor. All rigts reserved. Licensed under the MIT License (MIT).

using System.CommandLine;
using System.CommandLine.Help;
using System.CommandLine.IO;

namespace ClangSharp;

internal sealed class CustomHelpBuilder : HelpBuilder
{
    private IConsole Console { get; }

    public CustomHelpBuilder(IConsole console, LocalizationResources localizationResources,
        int maxWidth = int.MaxValue)
        : base(localizationResources, maxWidth)
    {
        Console = console;
    }

    public void Write(Option option)
    {
        Write(string.Join(", ", option.Aliases));
        Write("\t");
        Write(option.Description ?? "");
        WriteLine();
    }

    public void Write(string value) => Console.Out.Write(value);

    public void Write(params TwoColumnHelpRow[] helpItems)
    {
        WriteLine("Options:");
        var _ = new Command("unused");
        WriteColumns(helpItems, new HelpContext(this, _, Console.Out.CreateTextWriter()));
        WriteLine();
    }

    public void WriteLine() => Console.Out.WriteLine();
    
    public void WriteLine(string value) => Console.Out.WriteLine(value);
}
