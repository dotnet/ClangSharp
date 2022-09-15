// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;

namespace ClangSharp;

public sealed class CXXBaseSpecifier : Ref
{
    internal CXXBaseSpecifier(CXCursor handle) : base(handle, CXCursorKind.CXCursor_CXXBaseSpecifier)
    {
    }

    public CX_CXXAccessSpecifier AccessSpecifier => Handle.CXXAccessSpecifier;

    public bool IsVirtual => Handle.IsVirtualBase;
}
