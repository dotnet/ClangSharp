// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class FunctionParmPackExpr : Expr
{
    private readonly Lazy<IReadOnlyList<VarDecl>> _expansions;
    private readonly Lazy<VarDecl> _parameterPack;

    internal FunctionParmPackExpr(CXCursor handle) : base(handle, CXCursor_DeclRefExpr, CX_StmtClass_FunctionParmPackExpr)
    {
        Debug.Assert(NumChildren is 0);

        _expansions = new Lazy<IReadOnlyList<VarDecl>>(() => {
            var numExpansions = Handle.NumDecls;
            var expansions = new List<VarDecl>(numExpansions);

            for (var i = 0; i < numExpansions; i++)
            {
                var expansion = TranslationUnit.GetOrCreate<VarDecl>(Handle.GetDecl(unchecked((uint)i)));
                expansions.Add(expansion);
            }

            return expansions;
        });
        _parameterPack = new Lazy<VarDecl>(() => TranslationUnit.GetOrCreate<VarDecl>(Handle.Referenced));
    }

    public IReadOnlyList<VarDecl> Expansions => _expansions.Value;

    public uint NumExpansions => unchecked((uint)Handle.NumDecls);

    public VarDecl ParameterPack => _parameterPack.Value;
}
