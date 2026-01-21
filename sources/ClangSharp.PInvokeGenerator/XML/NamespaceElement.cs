// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Serialization;

namespace ClangSharp.XML;

[XmlType]
[DebuggerDisplay($"{{{nameof(Name)},nq}}")]
public sealed class NamespaceElement : NamedDescElement
{
    public NamespaceElement()
    {
        Structs = Types.OfType<StructDescElement>();
        Enums = Types.OfType<EnumDescElement>();
    }

    [XmlElement("struct", typeof(StructDescElement))]
    [XmlElement("enumeration", typeof(EnumDescElement))]
    [XmlElement("delegate", typeof(DelegateDescElement))]
    [XmlElement("class", typeof(MethodClassElement))]
    public List<NamedAccessDescElement> Types { get; } = [];

    [XmlIgnore]
    public IEnumerable<StructDescElement> Structs { get; }

    [XmlIgnore]
    public IEnumerable<EnumDescElement> Enums { get; }
}
