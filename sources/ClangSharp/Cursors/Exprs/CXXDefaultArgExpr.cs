// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp;

public sealed class CXXDefaultArgExpr : Expr
{
    private readonly Lazy<ParmVarDecl> _param;
    private readonly Lazy<IDeclContext?> _usedContext;

    internal CXXDefaultArgExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnexposedExpr, CX_StmtClass.CX_StmtClass_CXXDefaultArgExpr)
    {
        Debug.Assert(NumChildren is 0);

        _param = new Lazy<ParmVarDecl>(() => TranslationUnit.GetOrCreate<ParmVarDecl>(Handle.Referenced));
        _usedContext = new Lazy<IDeclContext?>(() => TranslationUnit.GetOrCreate<Decl>(Handle.UsedContext) as IDeclContext);
    }

    public Expr Expr => Param.DefaultArg;

    public ParmVarDecl Param => _param.Value;

    public IDeclContext? UsedContext => _usedContext.Value;
}
