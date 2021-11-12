// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

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
            if (handle.StmtClass is > CX_StmtClass.CX_StmtClass_LastCallExpr or < CX_StmtClass.CX_StmtClass_FirstCallExpr)
            {
                throw new ArgumentOutOfRangeException(nameof(handle));
            }

            Debug.Assert(NumChildren >= 1);

            _args = new Lazy<IReadOnlyList<Expr>>(() => Children.Skip(1).Take((int)NumArgs).Cast<Expr>().ToList());
            _calleeDecl = new Lazy<Decl>(() => TranslationUnit.GetOrCreate<Decl>(Handle.Referenced));
        }

        public IReadOnlyList<Expr> Args => _args.Value;

        public Expr Callee => (Expr)Children[0];

        public Decl CalleeDecl => _calleeDecl.Value;

        public FunctionDecl DirectCallee => CalleeDecl as FunctionDecl;

        public bool IsCallToStdMove => (NumArgs == 1) && (DirectCallee is FunctionDecl fd) && fd.IsInStdNamespace && (fd.Name == "move");

        public uint NumArgs => (uint)Handle.NumArguments;
    }
}
