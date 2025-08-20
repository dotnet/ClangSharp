// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Xml.Serialization;

namespace ClangSharp.XML;

[XmlType]
public class FunctionBodyElement
{
    [XmlElement("code")]
    public CSharpCodeElement? Code { get; init; }
}
