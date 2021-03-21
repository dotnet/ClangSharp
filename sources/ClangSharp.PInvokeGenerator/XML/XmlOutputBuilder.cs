// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using ClangSharp.Abstractions;

namespace ClangSharp.XML
{
    internal partial class XmlOutputBuilder : IOutputBuilder
    {
        public XmlOutputBuilder(string name)
        {
            Name = name;
        }

        public string Name { get; }
        public string Extension { get; } = ".xml";
        public bool IsTestOutput { get; } = false;

        public IEnumerable<string> Contents
        {
            get
            {
                StringWriter sw = new();
                XmlWriter writer = XmlWriter.Create(sw, new()
                {
                    Indent = true,
                    IndentChars = "  ",
                    ConformanceLevel = ConformanceLevel.Fragment,
                    NewLineChars = "\n",
                });

                foreach (var node in XElement.Parse("<tmp>" + _sb + "</tmp>").Nodes())
                {
                    node.WriteTo(writer);
                }

                writer.Flush();
                return sw.ToString().Split('\n');
            }
        }

        private string EscapeText(string value) => new XText(value).ToString();
    }
}
