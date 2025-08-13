// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public sealed class CXXDestructorDecl : CXXMethodDecl
{
    private readonly ValueLazy<FunctionDecl> _operatorDelete;
    private readonly ValueLazy<Expr> _operatorDeleteThisArg;

    internal CXXDestructorDecl(CXCursor handle) : base(handle, CXCursor_Destructor, CX_DeclKind_CXXDestructor)
    {
        _operatorDelete = new ValueLazy<FunctionDecl>(() => TranslationUnit.GetOrCreate<FunctionDecl>(Handle.GetSubDecl(0)));
        _operatorDeleteThisArg = new ValueLazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.GetExpr(0)));
    }

    public new CXXDestructorDecl CanonicalDecl => (CXXDestructorDecl)base.CanonicalDecl;

    public Expr OperatorDeleteThisArg => _operatorDeleteThisArg.Value;

    public FunctionDecl OperatorDelete => _operatorDelete.Value;
}
