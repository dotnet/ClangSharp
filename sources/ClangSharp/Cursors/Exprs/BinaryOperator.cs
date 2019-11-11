// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class BinaryOperator : Expr
    {
        private readonly Lazy<Expr> _lhs;
        private readonly Lazy<string> _opcode;
        private readonly Lazy<Expr> _rhs;

        internal BinaryOperator(CXCursor handle) : this(handle, CXCursorKind.CXCursor_BinaryOperator)
        {
        }

        private protected BinaryOperator(CXCursor handle, CXCursorKind expectedKind) : base(handle, expectedKind)
        {
            Debug.Assert(Children.Where((cursor) => cursor is Expr).Count() == 2);

            _lhs = new Lazy<Expr>(() => Children.OfType<Expr>().First());
            _opcode = new Lazy<string>(GetOpcode);
            _rhs = new Lazy<Expr>(() => Children.OfType<Expr>().Last());
        }

        public Expr LHS => _lhs.Value;

        public string Opcode => _opcode.Value;

        public Expr RHS => _rhs.Value;

        protected virtual string GetOpcode()
        {
            var lhsTokens = Handle.TranslationUnit.Tokenize(LHS.Extent);

            var tokens = Handle.TranslationUnit.Tokenize(Extent);
            Debug.Assert(tokens.Length >= 3);

            int operatorIndex = lhsTokens.Length;

            Debug.Assert(tokens[operatorIndex].Kind == CXTokenKind.CXToken_Punctuation);
            return tokens[operatorIndex].GetSpelling(Handle.TranslationUnit).ToString();
        }
    }
}
