// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.Xml.Serialization;

namespace ClangSharp.XML;

[XmlType]
public class VtblElement
{
    [XmlElement("field")]
    public List<FieldDescElement> Fields { get; } = [];
}
