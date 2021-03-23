// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class CallExpr : Expr
    {
        private readonly Lazy<IReadOnlyList<Expr>> _args;
        private readonly Lazy<Decl> _calleeDecl;

        internal CallExpr(CXCursor handle) : this(handle, CXCursorKind.CXCursor_CallExpr, CX_StmtClass.CX_StmtClass_CallExpr)
        {
        }

        private protected CallExpr(CXCursor handle, CXCursorKind expectedCursorKind, CX_StmtClass expectedStmtClass) : base(handle, expectedCursorKind, expectedStmtClass)
        {
            if ((CX_StmtClass.CX_StmtClass_LastCallExpr < handle.StmtClass) || (handle.StmtClass < CX_StmtClass.CX_StmtClass_FirstCallExpr))
            {
                throw new ArgumentException(nameof(handle));
            }

            Debug.Assert(NumChildren >= 1);

            _args = new Lazy<IReadOnlyList<Expr>>(() => Children.Skip(1).Take((int)NumArgs).Cast<Expr>().ToList());
            _calleeDecl = new Lazy<Decl>(() => TranslationUnit.GetOrCreate<Decl>(Handle.Referenced));
        }

        public IReadOnlyList<Expr> Args => _args.Value;

        public Expr Callee => (Expr)Children[0];

        public Decl CalleeDecl => _calleeDecl.Value;

        public FunctionDecl DirectCallee => CalleeDecl as FunctionDecl;

        public bool IsCallToStdMove => (NumArgs == 1) && (DirectCallee is FunctionDecl FD) && FD.IsInStdNamespace && (FD.Name == "move");

        public uint NumArgs => (uint)Handle.NumArguments;
    }
}
