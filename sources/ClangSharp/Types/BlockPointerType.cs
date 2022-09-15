// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;

namespace ClangSharp;

public sealed class BlockPointerType : Type
{
    internal BlockPointerType(CXType handle) : base(handle, CXTypeKind.CXType_BlockPointer, CX_TypeClass.CX_TypeClass_BlockPointer)
    {
    }
}
