// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

namespace ClangSharp.XML;

[XmlType]
public class StructDescElement : NamedAccessDescElement
{
    [XmlAttribute("native")]
    public string? NativeTypeName { get; init; }

    [XmlAttribute("parent")]
    public string? NativeInheritance { get; init; }

    [XmlAttribute("uuid")]
    public string? Uuid { get; init; }

    [XmlAttribute("vtbl")]
    public bool HasVtbl { get; init; }

    [XmlIgnore]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool HasVtblSpecified => HasVtbl;

    [XmlAttribute("unsafe")]
    public bool IsUnsafe { get; init; }

    [XmlIgnore]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool IsUnsafeSpecified => IsUnsafe;

    [XmlAttribute("layout")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public string? LayoutValue { get; init; }

    [XmlIgnore]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool LayoutValueSpecified => !string.IsNullOrEmpty(LayoutValue);

    [XmlIgnore]
    public LayoutKind? LayoutKind
    {
        get
        {
            return LayoutValueSpecified
                && Enum.TryParse(LayoutValue, out LayoutKind layout)
                ? layout
                : null;
        }
        init { LayoutValue = value.HasValue ? value.ToString() : null; }
    }

    [XmlAttribute("pack")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public string? PackValue { get; init; }

    [XmlIgnore]
    public int? Pack
    {
        get
        {
            return int.TryParse(PackValue, CultureInfo.InvariantCulture, out var pack)
                ? pack
                : null;
        }

        init
        {
            PackValue = value.HasValue
                ? value.Value.ToString(CultureInfo.InvariantCulture)
                : null;
        }
    }

    [XmlElement("attribute")]
    public List<string> Attributes { get; } = [];

    [XmlElement("constant", typeof(ConstantDescElement))]
    [XmlElement("field", typeof(FieldDescElement))]
    [XmlElement("struct", typeof(StructDescElement))]
    [XmlElement("indexer", typeof(IndexerDeclElement))]
    [XmlElement("function", typeof(FunctionDescElement))]
    [XmlElement("delegate", typeof(DelegateDescElement))]
    [XmlElement("interface", typeof(InterfaceElement))]
    [XmlElement("vtbl", typeof(VtblElement))]
    public List<object> Decls { get; } = [];
}
