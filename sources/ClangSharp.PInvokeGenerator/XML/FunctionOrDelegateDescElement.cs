// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;

namespace ClangSharp.XML;

[XmlType]
public abstract class FunctionOrDelegateDescElement : NamedAccessDescElement
{
    [XmlAttribute("lib")]
    public string? LibraryPath { get; init; }

    [XmlAttribute("convention")]
    public string? CallingConvention { get; init; }

    [XmlAttribute("entrypoint")]
    public string? EntryPoint { get; init; }

    [XmlAttribute("setlasterror")]
    public bool SetLastError { get; init; }

    [XmlIgnore]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool SetLastErrorSpecified => SetLastError;

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

    [XmlAttribute("vtblindex")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public string? VtblIndexValue { get; init; }

    [XmlIgnore]
    public long? VtblIndex
    {
        get
        {
            return !string.IsNullOrEmpty(VtblIndexValue)
                && long.TryParse(
                    VtblIndexValue,
                    CultureInfo.InvariantCulture,
                    out var vtblindex
                    ) ? vtblindex : null;
        }

        init
        {
            VtblIndexValue = value.HasValue
                ? value.Value.ToString(CultureInfo.InvariantCulture)
                : null;
        }
    }

    [XmlElement("attribute")]
    public List<string> Attributes { get; } = [];

    [XmlElement("type")]
    public required TypeWithNativeTypeNameElement ReturnType { get; init; }

    [XmlElement("param")]
    public List<ParameterDescElement> Parameters { get; } = [];
}
