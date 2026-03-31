// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.Xml.Serialization;

namespace ClangSharp.XML;

[XmlType]
public class ParameterDescElement : NamedDescElement
{
    [XmlElement("attribute")]
    public List<string> Attributes { get; } = [];

    [XmlElement("type")]
    public required string Type { get; init; }

    [XmlElement("init")]
    public ValueDescValueElement? DefaultValue { get; init; }
}
