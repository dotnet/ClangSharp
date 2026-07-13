// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using ClangSharp.Abstractions;

namespace ClangSharp.XML;

internal partial class XmlOutputBuilder(string name, PInvokeGenerator generator) : IOutputBuilder
{
    private readonly PInvokeGenerator _generator = generator;

    public string Name { get; } = name;
    public string Extension { get; } = ".xml";

    public bool IsUncheckedContext { get; private set; }

    public bool IsTestOutput { get; }

    private static readonly XmlWriterSettings s_writerSettings = new()
    {
        Indent = true,
        IndentChars = "  ",
        ConformanceLevel = ConformanceLevel.Fragment,
        NewLineChars = "\n",
    };

    public IEnumerable<string> Contents
    {
        get
        {
            using StringWriter sw = new();

            using (var writer = XmlWriter.Create(sw, s_writerSettings))
            {
                foreach (var node in XElement.Parse($"<tmp>{_sb}</tmp>").Nodes())
                {
                    node.WriteTo(writer);
                }
            }

            return sw.ToString().Split('\n');
        }
    }

    private static string EscapeText(string value)
    {
        var sb = new StringBuilder(value.Length);

        foreach (var c in value)
        {
            switch (c)
            {
                case '<':
                {
                    _ = sb.Append("&lt;");
                    break;
                }

                case '>':
                {
                    _ = sb.Append("&gt;");
                    break;
                }

                case '&':
                {
                    _ = sb.Append("&amp;");
                    break;
                }

                default:
                {
                    _ = sb.Append(c);
                    break;
                }
            }
        }

        return sb.ToString();
    }
}
