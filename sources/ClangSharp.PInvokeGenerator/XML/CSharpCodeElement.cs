// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace ClangSharp.XML;

[XmlType]
public sealed class CSharpCodeElement
{
    [XmlText]
    [XmlAnyElement]
    public List<XmlNode> Segments { get; } = [];
}
