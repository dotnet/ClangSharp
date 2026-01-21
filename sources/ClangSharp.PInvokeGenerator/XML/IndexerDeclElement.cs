// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.ComponentModel;
using System.Xml.Serialization;

namespace ClangSharp.XML;

[XmlType]
public class IndexerDeclElement
{
    [XmlAttribute("access")]
    public required string AccessSpecifier { get; init; }

    [XmlAttribute("unsafe")]
    public bool IsUnsafe { get; init; }

    [XmlIgnore]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool IsUnsafeSpecified => IsUnsafe;

    [XmlElement("type")]
    public required string Type { get; init; }

    [XmlElement("param")]
    public required ParameterDescElement IndexerParameter { get; init; }

    [XmlElement("get")]
    public required PropertyAccessorElement Getter { get; init; }
}
