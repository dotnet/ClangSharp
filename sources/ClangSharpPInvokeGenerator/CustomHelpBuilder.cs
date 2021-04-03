// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

// Portions of this code are ported from https://github.com/dotnet/command-line-api
// The original source is Copyright © .NET Foundation and Contributor. All rigts reserved. Licensed under the MIT License (MIT).

using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Help;
using System.CommandLine.IO;
using System.Linq;

namespace ClangSharp
{
    internal sealed class CustomHelpBuilder : HelpBuilder
    {
        public CustomHelpBuilder(IConsole console, int? columnGutter = null, int? indentationSize = null, int? maxWidth = null)
            : base(console, columnGutter, indentationSize, maxWidth)
        {
        }

        public void Write(IReadOnlyCollection<(string Name, string Description)> options)
        {
            if (options.Count == 0)
            {
                return;
            }

            AppendHeading("Options:");
            Indent();

            var table = CreateTable(options, (option) => new string[2] {
                option.Name,
                option.Description,
            });

            var columnWidths = ColumnWidths(table);
            AppendTable(table, columnWidths);

            Outdent();
            AppendBlankLine();
        }

        private void AppendBlankLine() => Console.Out.WriteLine();

        private void AppendHeading(string heading)
        {
            if (heading is null)
            {
                throw new ArgumentNullException(nameof(heading));
            }

            AppendLine(heading);
        }

        private void AppendLine(string text, int? offset = null)
        {
            if (string.IsNullOrEmpty(text))
            {
                Console.Out.WriteLine();
                return;
            }

            AppendPadding(offset);
            Console.Out.WriteLine(text);
        }

        private void AppendPadding(int? offset = null)
        {
            var padding = GetPadding(offset ?? CurrentIndentation);
            Console.Out.Write(padding);
        }

        private void AppendRow(IEnumerable<string> row, IReadOnlyList<int> columnWidths)
        {
            var array = row.Select((string element, int index) => SplitText(element, columnWidths[index])).ToArray();
            var num = array.Max((IReadOnlyList<string> lines) => lines.Count);

            for (var i = 0; i < num; i++)
            {
                var num2 = 0;
                var num3 = 0;

                AppendPadding(CurrentIndentation);

                for (var j = 0; j < array.Length; j++)
                {
                    var readOnlyList = array[j];

                    if (i < readOnlyList.Count)
                    {
                        var text = readOnlyList[i];

                        if (!string.IsNullOrEmpty(text))
                        {
                            var num4 = num2 - num3;
                            AppendText(text, num4);
                            num3 += num4 + text.Length;
                        }
                    }

                    num2 += columnWidths[j] + ColumnGutter;
                }

                AppendBlankLine();
            }
        }


        private void AppendTable(IEnumerable<IEnumerable<string>> table, IReadOnlyList<int> columnWidths)
        {
            foreach (var item in table)
            {
                AppendRow(item, columnWidths);
            }
        }

        private void AppendText(string text, int? offset = null)
        {
            AppendPadding(offset);
            Console.Out.Write(text ?? "");
        }

        private IReadOnlyList<int> ColumnWidths(IEnumerable<IReadOnlyList<string>> table)
        {
            if (!table.Any())
            {
                return Array.Empty<int>();
            }

            var count = table.First().Count;
            var num = -1;
            var array = new int[count];
            for (var j = 0; j < count; j++)
            {
                array[j] = num;
            }

            var array2 = new int[count];
            int i;
            for (i = 0; i < count; i++)
            {
                array2[i] = table.Max((IReadOnlyList<string> row) => row[i].Length);
            }

            var num2 = array2.Count((int width) => width > 0);
            var num3 = GetAvailableWidth() - (ColumnGutter * (num2 - 1));
            if (num3 - num2 < 0 || num2 == 0)
            {
                return array2;
            }

            var num4 = num2;
            var num5 = 0;
            var num6 = 5;
            while (num4 > 0)
            {
                var num7 = (num3 - array.Where((int width) => width > 0).Sum()) / num4;
                var flag = num4 == num5 || num6 <= 1;
                for (var k = 0; k < count; k++)
                {
                    if (array[k] == num)
                    {
                        var num8 = array2[k];
                        if (flag)
                        {
                            num8 = Math.Min(num8, num7);
                        }

                        if (num8 <= num7)
                        {
                            array[k] = num8;
                        }
                    }
                }

                num5 = num4;
                num4 = array.Count((int width) => width < 0);
                num6--;
            }

            return array;
        }
    }
}
