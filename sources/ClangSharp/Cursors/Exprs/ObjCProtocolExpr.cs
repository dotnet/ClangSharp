// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp;

public sealed class ObjCProtocolExpr : Expr
{
    private readonly Lazy<ObjCProtocolDecl> _protocol;

    internal ObjCProtocolExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_ObjCProtocolExpr, CX_StmtClass.CX_StmtClass_ObjCProtocolExpr)
    {
        Debug.Assert(NumChildren is 0);
        _protocol = new Lazy<ObjCProtocolDecl>(() => TranslationUnit.GetOrCreate<ObjCProtocolDecl>(Handle.Referenced));
    }

    public ObjCProtocolDecl Protocol => _protocol.Value;
}
