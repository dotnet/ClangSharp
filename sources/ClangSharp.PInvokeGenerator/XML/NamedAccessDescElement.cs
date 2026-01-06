// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.


using System.Xml.Serialization;

namespace ClangSharp.XML;

public abstract class NamedAccessDescElement : NamedDescElement
{
    [XmlAttribute("access")]
    public required string AccessSpecifier { get; init; }
}
