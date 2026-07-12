// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class ObjCBoxedExpr : Expr
{
    private ValueLazy<ObjCBoxedExpr, ObjCMethodDecl> _boxingMethod;

    internal unsafe ObjCBoxedExpr(CXCursor handle) : base(handle, CXCursor_UnexposedExpr, CX_StmtClass_ObjCBoxedExpr)
    {
        Debug.Assert(NumChildren is 1);
        _boxingMethod = new ValueLazy<ObjCBoxedExpr, ObjCMethodDecl>(&BoxingMethodFactory);
    }

    public ObjCMethodDecl BoxingMethod => _boxingMethod.GetValue(this);

    public Expr SubExpr => (Expr)Children[0];

    private static unsafe ObjCMethodDecl BoxingMethodFactory(ObjCBoxedExpr self) => self.TranslationUnit.GetOrCreate<ObjCMethodDecl>(self.Handle.Referenced);
}
