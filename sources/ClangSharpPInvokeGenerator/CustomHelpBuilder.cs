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

        private void AppendBlankLine()
        {
            Console.Out.WriteLine();
        }

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
            string padding = GetPadding(offset ?? CurrentIndentation);
            Console.Out.Write(padding);
        }

        private void AppendRow(IEnumerable<string> row, IReadOnlyList<int> columnWidths)
        {
            var array = row.Select((string element, int index) => SplitText(element, columnWidths[index])).ToArray();
            int num = array.Max((IReadOnlyList<string> lines) => lines.Count);

            for (int i = 0; i < num; i++)
            {
                int num2 = 0;
                int num3 = 0;

                AppendPadding(CurrentIndentation);

                for (int j = 0; j < array.Length; j++)
                {
                    var readOnlyList = array[j];

                    if (i < readOnlyList.Count)
                    {
                        string text = readOnlyList[i];

                        if (!string.IsNullOrEmpty(text))
                        {
                            int num4 = num2 - num3;
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
            foreach (IEnumerable<string> item in table)
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

            int count = table.First().Count;
            int num = -1;
            int[] array = new int[count];
            for (int j = 0; j < count; j++)
            {
                array[j] = num;
            }

            int[] array2 = new int[count];
            int i;
            for (i = 0; i < count; i++)
            {
                array2[i] = table.Max((IReadOnlyList<string> row) => row[i].Length);
            }

            int num2 = array2.Count((int width) => width > 0);
            int num3 = GetAvailableWidth() - ColumnGutter * (num2 - 1);
            if (num3 - num2 < 0 || num2 == 0)
            {
                return array2;
            }

            int num4 = num2;
            int num5 = 0;
            int num6 = 5;
            while (num4 > 0)
            {
                int num7 = (num3 - array.Where((int width) => width > 0).Sum()) / num4;
                bool flag = num4 == num5 || num6 <= 1;
                for (int k = 0; k < count; k++)
                {
                    if (array[k] == num)
                    {
                        int num8 = array2[k];
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
