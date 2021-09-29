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
        private readonly PInvokeGeneratorConfiguration _config;

        public XmlOutputBuilder(string name, PInvokeGeneratorConfiguration config)
        {
            Name = name;
            _config = config;
        }

        public string Name { get; }
        public string Extension { get; } = ".xml";

        public bool IsUncheckedContext { get; private set; }

        public bool IsTestOutput { get; } = false;

        public IEnumerable<string> Contents
        {
            get
            {
                StringWriter sw = new();
                var writer = XmlWriter.Create(sw, new()
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

        private static string EscapeText(string value) => new XText(value).ToString();
    }
}
