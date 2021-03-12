using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using ClangSharp.Abstractions;

namespace ClangSharp.XML
{
    public partial class XmlOutputBuilder : IOutputBuilder
    {
        public XmlOutputBuilder(string name)
        {
            Name = name;
        }

        public string Name { get; }
        public string Extension { get; } = ".xml";
        public bool IsTestOutput { get; } = false;

        public IEnumerable<string> Contents => XElement.Parse("<tmp>" + _sb + "</tmp>")
            .Nodes()
            .SelectMany(x => x.ToString().Split('\n'));
    }
}
