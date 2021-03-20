using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                    IndentChars = "    ",
                    ConformanceLevel = ConformanceLevel.Fragment
                });

                foreach (var node in XElement.Parse("<tmp>" + _sb + "</tmp>").Nodes())
                {
                    node.WriteTo(writer);
                }

                writer.Flush();
                return sw.ToString().Split('\n');
            }
        }
    }
}
