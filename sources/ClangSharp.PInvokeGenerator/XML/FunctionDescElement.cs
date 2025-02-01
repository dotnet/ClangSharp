// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.Xml.Serialization;

namespace ClangSharp.XML;

[XmlType]
public class FunctionDescElement : FunctionOrDelegateDescElement
{
    [XmlElement("init")]
    public List<TypeInitializationExpressionElement> TypeInitializationExpressions { get; } = [];

    [XmlElement("body")]
    public FunctionBodyElement? BlockBody { get; init; }

    [XmlElement("code")]
    public CSharpCodeElement? ExpressionBody { get; init; }
}
