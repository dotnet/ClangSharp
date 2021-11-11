// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class CXXUnresolvedConstructExpr : Expr
    {
        private readonly Lazy<IReadOnlyList<Expr>> _args;
        private readonly Lazy<Type> _typeAsWritten;

        internal CXXUnresolvedConstructExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_CallExpr, CX_StmtClass.CX_StmtClass_CXXUnresolvedConstructExpr)
        {
            _args = new Lazy<IReadOnlyList<Expr>>(() => Children.Cast<Expr>().ToList());
            _typeAsWritten = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.TypeOperand));
        }

        public IReadOnlyList<Expr> Args => _args.Value;

        public bool IsListInitialization => Handle.IsListInitialization;

        public uint NumArgs => unchecked((uint)Handle.NumArguments);

        public Type TypeAsWritten => _typeAsWritten.Value;
    }
}
