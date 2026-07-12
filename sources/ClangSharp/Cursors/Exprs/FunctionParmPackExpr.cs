// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_StmtClass;
using static ClangSharp.Interop.CXCursorKind;

namespace ClangSharp;

public sealed class FunctionParmPackExpr : Expr
{
    private readonly LazyList<VarDecl> _expansions;
    private ValueLazy<FunctionParmPackExpr, VarDecl> _parameterPack;

    internal unsafe FunctionParmPackExpr(CXCursor handle) : base(handle, CXCursor_DeclRefExpr, CX_StmtClass_FunctionParmPackExpr)
    {
        Debug.Assert(NumChildren is 0);

        _expansions = LazyList.Create<VarDecl>(this, Handle.NumDecls, &ExpansionsFactory);
        _parameterPack = new ValueLazy<FunctionParmPackExpr, VarDecl>(&ParameterPackFactory);
    }

    public IReadOnlyList<VarDecl> Expansions => _expansions;

    public uint NumExpansions => unchecked((uint)Handle.NumDecls);

    public VarDecl ParameterPack => _parameterPack.GetValue(this);

    private static unsafe VarDecl ParameterPackFactory(FunctionParmPackExpr self) => self.TranslationUnit.GetOrCreate<VarDecl>(self.Handle.Referenced);

    private static unsafe VarDecl ExpansionsFactory(object self, int i)
    {
        var @this = (FunctionParmPackExpr)self;
        return @this.TranslationUnit.GetOrCreate<VarDecl>(@this.Handle.GetDecl(unchecked((uint)i)));
    }
}
