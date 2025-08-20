// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Xml.Serialization;

namespace ClangSharp.XML;

[XmlType]
public class TypeInitializationExpressionElement : FunctionBodyElement
{
    [XmlAttribute("field")]
    public required string Field { get; init; }

    [XmlAttribute("hint")]
    public string? Hint { get; init; }
}
