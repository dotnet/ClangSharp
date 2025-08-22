// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class CXXDefaultArgExpr : Expr
{
    private readonly ValueLazy<ParmVarDecl> _param;
    private readonly ValueLazy<IDeclContext?> _usedContext;

    internal CXXDefaultArgExpr(CXCursor handle) : base(handle, CXCursor_UnexposedExpr, CX_StmtClass_CXXDefaultArgExpr)
    {
        Debug.Assert(NumChildren is 0);

        _param = new ValueLazy<ParmVarDecl>(() => TranslationUnit.GetOrCreate<ParmVarDecl>(Handle.Referenced));
        _usedContext = new ValueLazy<IDeclContext?>(() => TranslationUnit.GetOrCreate<Decl>(Handle.UsedContext) as IDeclContext);
    }

    public ParmVarDecl Param => _param.Value;

    public IDeclContext? UsedContext => _usedContext.Value;
}
