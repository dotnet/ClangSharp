// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class CXXDefaultArgExpr : Expr
{
    private ValueLazy<CXXDefaultArgExpr, ParmVarDecl> _param;
    private ValueLazy<CXXDefaultArgExpr, IDeclContext?> _usedContext;

    internal unsafe CXXDefaultArgExpr(CXCursor handle) : base(handle, CXCursor_UnexposedExpr, CX_StmtClass_CXXDefaultArgExpr)
    {
        Debug.Assert(NumChildren is 0);

        _param = new ValueLazy<CXXDefaultArgExpr, ParmVarDecl>(&ParamFactory);
        _usedContext = new ValueLazy<CXXDefaultArgExpr, IDeclContext?>(&UsedContextFactory);
    }

    public ParmVarDecl Param => _param.GetValue(this);

    public IDeclContext? UsedContext => _usedContext.GetValue(this);

    private static unsafe IDeclContext? UsedContextFactory(CXXDefaultArgExpr self) => self.TranslationUnit.GetOrCreate<Decl>(self.Handle.UsedContext) as IDeclContext;

    private static unsafe ParmVarDecl ParamFactory(CXXDefaultArgExpr self) => self.TranslationUnit.GetOrCreate<ParmVarDecl>(self.Handle.Referenced);
}
