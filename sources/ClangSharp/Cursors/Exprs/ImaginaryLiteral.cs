// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class ImaginaryLiteral : Expr
{
    internal ImaginaryLiteral(CXCursor handle) : base(handle, CXCursor_ImaginaryLiteral, CX_StmtClass_ImaginaryLiteral)
    {
    }
}
