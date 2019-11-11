// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class UnaryOperator : Expr
    {
        private readonly Lazy<(string Opcode, bool IsPrefix)> _opcode;
        private readonly Lazy<Expr> _subExpr;

        internal UnaryOperator(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnaryOperator)
        {
            _opcode = new Lazy<(string Opcode, bool IsPrefix)>(GetOpcode);
            _subExpr = new Lazy<Expr>(() => Children.Where((cursor) => cursor is Expr).Cast<Expr>().Single());
        }

        public bool IsPrefix => _opcode.Value.IsPrefix;

        public bool IsPostfix => !_opcode.Value.IsPrefix;

        public string Opcode => _opcode.Value.Opcode;

        public Expr SubExpr => _subExpr.Value;

        private (string Opcode, bool IsPrefix) GetOpcode()
        {
            var subExprTokens = Handle.TranslationUnit.Tokenize(SubExpr.Extent);

            var tokens = Handle.TranslationUnit.Tokenize(Extent);
            Debug.Assert(tokens.Length >= 2);

            bool isPrefix = tokens[0] != subExprTokens[0];
            int operatorIndex = isPrefix ? 0 : subExprTokens.Length;

            Debug.Assert(tokens[operatorIndex].Kind == CXTokenKind.CXToken_Punctuation);
            var opcode = tokens[operatorIndex].GetSpelling(Handle.TranslationUnit).ToString();
            return (opcode, isPrefix);
        }
    }
}
