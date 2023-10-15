// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using ClangSharp.Abstractions;

namespace ClangSharp.XML;

internal partial class XmlOutputBuilder(string name, PInvokeGeneratorConfiguration config) : IOutputBuilder
{
    private readonly PInvokeGeneratorConfiguration _config = config;

    public string Name { get; } = name;
    public string Extension { get; } = ".xml";

    public bool IsUncheckedContext { get; private set; }

    public bool IsTestOutput { get; }

    public IEnumerable<string> Contents
    {
        get
        {
            StringWriter sw = new();
            using var writer = XmlWriter.Create(sw, new()
            {
                Indent = true,
                IndentChars = "  ",
                ConformanceLevel = ConformanceLevel.Fragment,
                NewLineChars = "\n",
            });

            foreach (var node in XElement.Parse($"<tmp>{_sb}</tmp>").Nodes())
            {
                node.WriteTo(writer);
            }

            writer.Flush();
            return sw.ToString().Split('\n');
        }
    }

    private static string EscapeText(string value) => new XText(value).ToString();
}
