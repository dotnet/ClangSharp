// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class UnaryExprOrTypeTraitExpr : Expr
    {
        private readonly Lazy<Expr> _argumentExpr;
        private readonly Lazy<CX_UnaryExprOrTypeTrait> _kind;

        internal UnaryExprOrTypeTraitExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnaryExpr, CX_StmtClass.CX_StmtClass_UnaryExprOrTypeTraitExpr)
        {
            _argumentExpr = new Lazy<Expr>(() => CursorChildren.OfType<Expr>().Single());
            _kind = new Lazy<CX_UnaryExprOrTypeTrait>(() => {
                var translationUnitHandle = TranslationUnit.Handle;
                
                var tokens = translationUnitHandle.Tokenize(Handle.RawExtent);
                var firstTokenSpelling = (tokens.Length > 0) ? tokens[0].GetSpelling(translationUnitHandle).CString : string.Empty;

                switch (firstTokenSpelling)
                {
                    case "sizeof":
                    {
                        return CX_UnaryExprOrTypeTrait.CX_UETT_SizeOf;
                    }

                    default:
                    {
                        return CX_UnaryExprOrTypeTrait.CX_UETT_Invalid;
                    }
                }
            });
        }

        public Expr ArgumentExpr => _argumentExpr.Value;

        public CX_UnaryExprOrTypeTrait Kind => _kind.Value;

        public Type TypeOfArgument => ArgumentExpr.Type;
    }
}
