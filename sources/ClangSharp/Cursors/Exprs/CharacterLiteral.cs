// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class CharacterLiteral : Expr
    {
        private readonly Lazy<string> _valueString;

        internal CharacterLiteral(CXCursor handle) : base(handle, CXCursorKind.CXCursor_CharacterLiteral, CX_StmtClass.CX_StmtClass_CharacterLiteral)
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

        public CX_CharacterKind Kind => Handle.CharacterLiteralKind;

        public uint Value => Handle.CharacterLiteralValue;

        public string ValueString => _valueString.Value;
    }
}
