// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_StmtClass;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CXTokenKind;

namespace ClangSharp;

public sealed class CXXBoolLiteralExpr : Expr
{
    private ValueLazy<CXXBoolLiteralExpr, string> _valueString;

    internal unsafe CXXBoolLiteralExpr(CXCursor handle) : base(handle, CXCursor_CXXBoolLiteralExpr, CX_StmtClass_CXXBoolLiteralExpr)
    {
        Debug.Assert(NumChildren is 0);

        _valueString = new ValueLazy<CXXBoolLiteralExpr, string>(&ValueStringFactory);
    }

    public bool Value => Handle.BoolLiteralValue;

    public string ValueString => _valueString.GetValue(this);

    private static unsafe string ValueStringFactory(CXXBoolLiteralExpr self) {
            var tokens = self.Handle.TranslationUnit.Tokenize(self.Handle.SourceRange);

            if ((tokens.Length == 0) || (tokens[0].Kind is not CXToken_Keyword and not CXToken_Identifier))
            {
                tokens = self.Handle.TranslationUnit.Tokenize(self.Handle.SourceRangeRaw);

                if ((tokens.Length == 0) || (tokens[0].Kind is not CXToken_Keyword and not CXToken_Identifier))
                {
                    Debug.Assert(false, "Failed to stringify tokens for CXX bool literal.");
                    return self.Value.ToString();
                }
            }

            var spelling = tokens[0].GetSpelling(self.Handle.TranslationUnit).ToString();
            spelling = spelling.Trim('\\', '\r', '\n');
            return spelling;
        }
}
