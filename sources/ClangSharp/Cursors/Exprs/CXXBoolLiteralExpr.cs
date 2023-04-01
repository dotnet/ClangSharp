// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CXTokenKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class CXXBoolLiteralExpr : Expr
{
    private readonly Lazy<string> _valueString;

    internal CXXBoolLiteralExpr(CXCursor handle) : base(handle, CXCursor_CXXBoolLiteralExpr, CX_StmtClass_CXXBoolLiteralExpr)
    {
        Debug.Assert(NumChildren is 0);

        _valueString = new Lazy<string>(() => {
            var tokens = Handle.TranslationUnit.Tokenize(Handle.SourceRange);

            Debug.Assert(tokens.Length == 1);
            Debug.Assert(tokens[0].Kind == CXToken_Keyword);

            var spelling = tokens[0].GetSpelling(Handle.TranslationUnit).ToString();
            spelling = spelling.Trim('\\', '\r', '\n');
            return spelling;
        });
    }

    public bool Value => Handle.BoolLiteralValue;

    public string ValueString => _valueString.Value;
}
