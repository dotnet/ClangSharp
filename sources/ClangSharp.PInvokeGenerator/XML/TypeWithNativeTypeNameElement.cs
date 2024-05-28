// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace ClangSharp.XML;

[XmlType]
[DebuggerDisplay($"{{{nameof(TypeName)},nq}}")]
public class TypeWithNativeTypeNameElement
{
    [XmlAttribute("primitive")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public string? PrimitiveValue { get; init; }

    [XmlIgnore]
    public bool IsPrimitive
    {
        get
        {
            _ = bool.TryParse(PrimitiveValue, out var primitive);
            return primitive;
        }

        init
        {
            PrimitiveValue = value.ToString();
        }
    }

    [XmlAttribute("native")]
    public string? NativeTypeName { get; init; }

    [XmlText]
    public required string TypeName { get; init; }
}
