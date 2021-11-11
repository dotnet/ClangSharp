// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

// Portions of this code are ported from https://github.com/dotnet/command-line-api
// The original source is Copyright © .NET Foundation and Contributor. All rigts reserved. Licensed under the MIT License (MIT).

using System.CommandLine;
using System.CommandLine.Help;
using System.CommandLine.IO;

namespace ClangSharp
{
    internal sealed class CustomHelpBuilder : HelpBuilder
    {
        public CustomHelpBuilder(IConsole console, int maxWidth = int.MaxValue)
            : base(console, maxWidth)
        {
        }

        public void Write(IOption option)
        {
            Write(string.Join(", ", option.Aliases));
            Write("\t");
            Write(option.Description);
            WriteLine();
        }

        public void Write(string value) => Console.Out.Write(value);

        public void Write(params HelpItem[] helpItems)
        {
            WriteLine("Options:");
            RenderAsColumns(helpItems);
            WriteLine();
        }

        public void WriteLine() => Console.Out.WriteLine();

        public void WriteLine(string value) => Console.Out.WriteLine(value);
    }
}
