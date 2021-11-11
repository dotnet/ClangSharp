// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class FunctionParmPackExpr : Expr
    {
        private readonly Lazy<IReadOnlyList<VarDecl>> _expansions;
        private readonly Lazy<VarDecl> _parameterPack;

        internal FunctionParmPackExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_DeclRefExpr, CX_StmtClass.CX_StmtClass_FunctionParmPackExpr)
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
}
