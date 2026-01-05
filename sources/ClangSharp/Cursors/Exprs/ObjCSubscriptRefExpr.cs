// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class ObjCSubscriptRefExpr : Expr
{
    private readonly ValueLazy<ObjCMethodDecl> _atIndexMethodDecl;

    internal ObjCSubscriptRefExpr(CXCursor handle) : base(handle, CXCursor_UnexposedExpr, CX_StmtClass_ObjCSubscriptRefExpr)
    {
        Debug.Assert(NumChildren is 2);
        _atIndexMethodDecl = new ValueLazy<ObjCMethodDecl>(() => TranslationUnit.GetOrCreate<ObjCMethodDecl>(Handle.Referenced));
    }

    public ObjCMethodDecl AtIndexMethodDecl => _atIndexMethodDecl.Value;

    public Expr BaseExpr => (Expr)Children[0];

    public Expr KeyExpr => (Expr)Children[1];

    public bool IsArraySubscriptRefExpr => KeyExpr.Type.IsIntegralOrEnumerationType;
}
