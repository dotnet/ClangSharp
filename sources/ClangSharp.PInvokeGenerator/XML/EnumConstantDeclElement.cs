// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using System.Xml.Serialization;

namespace ClangSharp.XML;

[XmlType]
[DebuggerDisplay($"{{{nameof(Name)},nq}}")]
public class EnumConstantDeclElement : FieldDescElement { }
