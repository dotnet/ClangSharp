// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Xml.Serialization;

namespace ClangSharp.XML;

[XmlType]
public class PropertyAccessorElement
{
    [XmlAttribute("inlining")]
    public string? Inlining { get; init; }

    [XmlElement("code")]
    public required CSharpCodeElement Code { get; init; }
}
