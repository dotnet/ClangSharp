// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

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

                var spelling = tokens[0].GetSpelling(Handle.TranslationUnit).ToString();
                spelling = spelling.Trim('\\', '\r', '\n');
                return spelling;
            });
        }

        public CX_FloatingSemantics RawSemantics => Handle.FloatingLiteralSemantics;

        public double ValueAsApproximateDouble => Handle.FloatingLiteralValueAsApproximateDouble;

        public string ValueString => _valueString.Value;
    }
}
