// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace ClangSharp.XML;

[XmlType]
[XmlRoot("bindings")]
public sealed class BindingsElement
{
    [XmlElement("comment")]
    public string? HeaderText { get; init; }

    [XmlElement("namespace")]
    public List<NamespaceElement> Namespaces { get; } = [];
}
