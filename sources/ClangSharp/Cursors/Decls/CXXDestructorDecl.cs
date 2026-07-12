// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public sealed class CXXDestructorDecl : CXXMethodDecl
{
    private ValueLazy<CXXDestructorDecl, FunctionDecl> _operatorDelete;
    private ValueLazy<CXXDestructorDecl, Expr> _operatorDeleteThisArg;

    internal unsafe CXXDestructorDecl(CXCursor handle) : base(handle, CXCursor_Destructor, CX_DeclKind_CXXDestructor)
    {
        _operatorDelete = new ValueLazy<CXXDestructorDecl, FunctionDecl>(&OperatorDeleteFactory);
        _operatorDeleteThisArg = new ValueLazy<CXXDestructorDecl, Expr>(&OperatorDeleteThisArgFactory);
    }

    public new CXXDestructorDecl CanonicalDecl => (CXXDestructorDecl)base.CanonicalDecl;

    public Expr OperatorDeleteThisArg => _operatorDeleteThisArg.GetValue(this);

    public FunctionDecl OperatorDelete => _operatorDelete.GetValue(this);

    private static unsafe Expr OperatorDeleteThisArgFactory(CXXDestructorDecl self) => self.TranslationUnit.GetOrCreate<Expr>(self.Handle.GetExpr(0));

    private static unsafe FunctionDecl OperatorDeleteFactory(CXXDestructorDecl self) => self.TranslationUnit.GetOrCreate<FunctionDecl>(self.Handle.GetSubDecl(0));
}
