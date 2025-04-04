// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.
// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
    "Design",
    "CA1002: Do not expose generic lists",
    Scope = "namespaceAndDescendants",
    Target = "~N:ClangSharp.XML",
    Justification = nameof(System.Xml.Serialization)
    )]

