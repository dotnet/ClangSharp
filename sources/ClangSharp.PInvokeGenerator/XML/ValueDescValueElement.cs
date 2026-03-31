// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Xml.Serialization;

namespace ClangSharp.XML;

[XmlType]
public class ValueDescValueElement
{
    [XmlElement("unchecked")]
    public ValueDescValueElement? Unchecked { get; init; }

    [XmlElement("cast")]
    public string? CastTargetTypeName { get; init; }

    [XmlElement("value")]
    public ValueDescValueElement? NestedValue { get; init; }

    [XmlElement("code")]
    public CSharpCodeElement? Code { get; init; }
}
