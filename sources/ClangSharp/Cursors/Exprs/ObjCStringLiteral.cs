// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class ObjCStringLiteral : Expr
{
    internal ObjCStringLiteral(CXCursor handle) : base(handle, CXCursor_ObjCStringLiteral, CX_StmtClass_ObjCStringLiteral)
    {
        Debug.Assert(NumChildren is 1);
    }

    public StringLiteral String => (StringLiteral)Children[0];
}
