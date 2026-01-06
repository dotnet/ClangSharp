// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Xml.Serialization;

namespace ClangSharp.XML;

[XmlType]
[DebuggerDisplay($"{{{nameof(AccessSpecifier)},nq}} {{{nameof(Type)}}} {{{nameof(Name)},nq}}")]
public class FieldDescElement : NamedAccessDescElement
{
    [XmlAttribute("inherited")]
    public string? InheritedFrom { get; init; }

    [XmlAttribute("offset")]
    public string? OffsetValue { get; init; }

    [XmlIgnore]
    public int? Offset
    {
        get
        {
            return int.TryParse(
                OffsetValue,
                CultureInfo.InvariantCulture,
                out var offset
                ) ? offset : null;
        }

        init
        {
            OffsetValue = value.HasValue
                ? value.Value.ToString(CultureInfo.InvariantCulture)
                : null;
        }
    }

    [XmlElement("attribute")]
    public List<string> Attributes { get; } = [];

    [XmlElement("type")]
    public required FieldTypeElement Type { get; init; }

    [XmlElement("value")]
    public ValueDescValueElement? Initializer { get; init; }

    [XmlElement("get")]
    public PropertyAccessorElement? Getter { get; init; }

    [XmlElement("set")]
    public PropertyAccessorElement? Setter { get; init; }
}
