// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace ClangSharp.XML;

[XmlType]
public class MethodClassElement : NamedAccessDescElement
{
    [XmlAttribute("static")]
    public bool IsStatic { get; init; }

    [XmlIgnore]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool IsStaticSpecified => IsStatic;

    [XmlAttribute("unsafe")]
    public bool IsUnsafe { get; init; }

    [XmlIgnore]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool IsUnsafeSpecified => IsUnsafe;

    [XmlElement("constant", typeof(ConstantDescElement))]
    [XmlElement("field", typeof(FieldDescElement))]
    [XmlElement("function", typeof(FunctionDescElement))]
    [XmlElement("delegate", typeof(DelegateDescElement))]
    [XmlElement("iid", typeof(IidDescElement))]
    public List<NamedDescElement> Decls { get; } = [];
}
