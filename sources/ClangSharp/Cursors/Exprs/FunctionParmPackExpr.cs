// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_StmtClass;
using static ClangSharp.Interop.CXCursorKind;

namespace ClangSharp;

public sealed class FunctionParmPackExpr : Expr
{
    private readonly LazyList<VarDecl> _expansions;
    private readonly ValueLazy<VarDecl> _parameterPack;

    internal FunctionParmPackExpr(CXCursor handle) : base(handle, CXCursor_DeclRefExpr, CX_StmtClass_FunctionParmPackExpr)
    {
        Debug.Assert(NumChildren is 0);

        _expansions = LazyList.Create<VarDecl>(Handle.NumDecls, (i) => TranslationUnit.GetOrCreate<VarDecl>(Handle.GetDecl(unchecked((uint)i))));
        _parameterPack = new ValueLazy<VarDecl>(() => TranslationUnit.GetOrCreate<VarDecl>(Handle.Referenced));
    }

    public IReadOnlyList<VarDecl> Expansions => _expansions;

    public uint NumExpansions => unchecked((uint)Handle.NumDecls);

    public VarDecl ParameterPack => _parameterPack.Value;
}
