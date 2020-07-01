// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class FloatingLiteral : Expr
    {
        private readonly Lazy<string> _valueString;

        internal FloatingLiteral(CXCursor handle) : base(handle, CXCursorKind.CXCursor_FloatingLiteral, CX_StmtClass.CX_StmtClass_FloatingLiteral)
        {
            _valueString = new Lazy<string>(() => {
                var tokens = Handle.TranslationUnit.Tokenize(Handle.SourceRange);

                Debug.Assert(tokens.Length == 1);
                Debug.Assert(tokens[0].Kind == CXTokenKind.CXToken_Literal);

                return tokens[0].GetSpelling(Handle.TranslationUnit).ToString();
            });
        }

        public CX_FloatingSemantics RawSemantics => Handle.FloatingLiteralSemantics;

        public double ValueAsApproximateDouble => Handle.FloatingLiteralValueAsApproximateDouble;

        public string ValueString => _valueString.Value;
    }
}
