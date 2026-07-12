// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;

namespace ClangSharp.Abstractions;

internal struct ValueDesc
{
    public AccessSpecifier AccessSpecifier { get; set; }
    public string TypeName { get; set; }
    public string EscapedName { get; set; }
    public string? NativeTypeName { get; set; }

    public string ParentName { get; set; }
    public ValueKind Kind { get; set; }
    public ValueFlags Flags { get; set; }
    public CXSourceLocation? Location { get; set; }

    public readonly bool HasInitializer => (Flags & ValueFlags.Initializer) != 0;
    public readonly bool IsArray => (Flags & ValueFlags.Array) != 0;
    public readonly bool IsConstant => (Flags & ValueFlags.Constant) != 0;
    public readonly bool IsCopy => (Flags & ValueFlags.Copy) != 0;
    public Action<object> WriteCustomAttrs { get; set; }
    public object CustomAttrGeneratorData { get; set; }
}
