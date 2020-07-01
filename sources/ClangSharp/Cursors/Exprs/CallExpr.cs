// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class CallExpr : Expr
    {
        private readonly Lazy<IReadOnlyList<Expr>> _args;
        private readonly Lazy<Expr> _callee;
        private readonly Lazy<Decl> _calleeDecl;
        private readonly Lazy<FunctionDecl> _directCallee;

        internal CallExpr(CXCursor handle) : this(handle, CXCursorKind.CXCursor_CallExpr, CX_StmtClass.CX_StmtClass_CallExpr)
        {
        }

        private protected CallExpr(CXCursor handle, CXCursorKind expectedCursorKind, CX_StmtClass expectedStmtClass) : base(handle, expectedCursorKind, expectedStmtClass)
        {
            if ((CX_StmtClass.CX_StmtClass_LastCallExpr < handle.StmtClass) || (handle.StmtClass < CX_StmtClass.CX_StmtClass_FirstCallExpr))
            {
                throw new ArgumentException(nameof(handle));
            }

            _args = new Lazy<IReadOnlyList<Expr>>(() => {
                var numArgs = Handle.NumArguments;
                var args = new List<Expr>(numArgs);

                for (var index = 0; index < numArgs; index++)
                {
                    var arg = TranslationUnit.GetOrCreate<Expr>(Handle.GetArgument(unchecked((uint)index)));
                    args.Add(arg);
                }

                return args;
            });
            _callee = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.CalleeExpr));
            _calleeDecl = new Lazy<Decl>(() => TranslationUnit.GetOrCreate<Decl>(Handle.Referenced));
            _directCallee = new Lazy<FunctionDecl>(() => TranslationUnit.GetOrCreate<FunctionDecl>(Handle.DirectCallee));
        }

        public IReadOnlyList<Expr> Args => _args.Value;

        public Expr Callee => _callee.Value;

        public Decl CalleeDecl => _calleeDecl.Value;

        public FunctionDecl DirectCallee => _directCallee.Value;

        public uint NumArgs => (uint)Handle.NumArguments;
    }
}
