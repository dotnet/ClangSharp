// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class CXXBoolLiteralExpr : Expr
    {
        private readonly Lazy<string> _value;

        internal CXXBoolLiteralExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_CXXBoolLiteralExpr, CX_StmtClass.CX_StmtClass_CXXBoolLiteralExpr)
        {
            _value = new Lazy<string>(() => {
                var tokens = Handle.TranslationUnit.Tokenize(Extent);

                Debug.Assert(tokens.Length == 1);
                Debug.Assert(tokens[0].Kind == CXTokenKind.CXToken_Keyword);

                return tokens[0].GetSpelling(Handle.TranslationUnit).ToString();
            });
        }

        public string Value => _value.Value;
    }
}
