// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;

namespace ClangSharp.Abstractions;

internal struct FieldDesc
{
    public AccessSpecifier AccessSpecifier { get; set; }
    public string? NativeTypeName { get; set; }
    public string EscapedName { get; set; }
    public string ParentName { get; set; }
    public int? Offset { get; set; }
    public bool NeedsNewKeyword { get; set; }
    public bool NeedsUnscopedRef { get; set; }
    public bool HasBody { get; set; }
    public string InheritedFrom { get; set; }
    public CXSourceLocation? Location { get; set; }
    public Action<object> WriteCustomAttrs { get; set; }
    public object CustomAttrGeneratorData { get; set; }
}
