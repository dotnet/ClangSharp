// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class ObjCSubscriptRefExpr : Expr
{
    private ValueLazy<ObjCSubscriptRefExpr, ObjCMethodDecl> _atIndexMethodDecl;
    private ValueLazy<ObjCSubscriptRefExpr, ObjCMethodDecl> _setAtIndexMethodDecl;

    internal unsafe ObjCSubscriptRefExpr(CXCursor handle) : base(handle, CXCursor_UnexposedExpr, CX_StmtClass_ObjCSubscriptRefExpr)
    {
        Debug.Assert(NumChildren is 2);
        _atIndexMethodDecl = new ValueLazy<ObjCSubscriptRefExpr, ObjCMethodDecl>(&AtIndexMethodDeclFactory);
        _setAtIndexMethodDecl = new ValueLazy<ObjCSubscriptRefExpr, ObjCMethodDecl>(&SetAtIndexMethodDeclFactory);
    }

    public ObjCMethodDecl AtIndexMethodDecl => _atIndexMethodDecl.GetValue(this);

    public Expr BaseExpr => (Expr)Children[0];

    public Expr KeyExpr => (Expr)Children[1];

    public bool IsArraySubscriptRefExpr => KeyExpr.Type.IsIntegralOrEnumerationType;

    public ObjCMethodDecl SetAtIndexMethodDecl => _setAtIndexMethodDecl.GetValue(this);

    private static unsafe ObjCMethodDecl AtIndexMethodDeclFactory(ObjCSubscriptRefExpr self) => self.TranslationUnit.GetOrCreate<ObjCMethodDecl>(self.Handle.Referenced);

    private static unsafe ObjCMethodDecl SetAtIndexMethodDeclFactory(ObjCSubscriptRefExpr self) => self.TranslationUnit.GetOrCreate<ObjCMethodDecl>(self.Handle.SetAtIndexMethodDecl);
}
