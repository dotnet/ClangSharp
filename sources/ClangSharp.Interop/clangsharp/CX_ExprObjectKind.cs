// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.
// Ported from https://github.com/dotnet/clangsharp/blob/main/sources/libClangSharp

namespace ClangSharp.Interop;

public enum CX_ExprObjectKind
{
    CX_OK_Invalid,
    CX_OK_Ordinary = 1,
    CX_OK_BitField = 2,
    CX_OK_VectorComponent = 3,
    CX_OK_ObjCProperty = 4,
    CX_OK_ObjCSubscript = 5,
    CX_OK_MatrixComponent = 6,
}
