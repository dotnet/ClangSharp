// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public sealed class ConstantMatrixType : MatrixType
{
    internal ConstantMatrixType(CXType handle) : base(handle, CXType_Unexposed, CX_TypeClass_ConstantMatrix)
    {
    }

    public uint NumColumns => unchecked((uint)Handle.NumColumns);

    public uint NumElementsFlattened => unchecked((uint)Handle.NumElementsFlattened);

    public uint NumRows => unchecked((uint)Handle.NumRows);
}
