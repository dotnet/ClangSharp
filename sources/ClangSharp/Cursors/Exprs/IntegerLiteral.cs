// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp;

public sealed class IntegerLiteral : Expr
{
    private readonly Lazy<string> _valueString;

    internal IntegerLiteral(CXCursor handle) : base(handle, CXCursorKind.CXCursor_IntegerLiteral, CX_StmtClass.CX_StmtClass_IntegerLiteral)
    {
        _valueString = new Lazy<string>(() => {
            var tokens = Handle.TranslationUnit.Tokenize(Handle.SourceRange);

            Debug.Assert(tokens.Length == 1);
            Debug.Assert(tokens[0].Kind is CXTokenKind.CXToken_Literal or CXTokenKind.CXToken_Identifier);

            var spelling = tokens[0].GetSpelling(Handle.TranslationUnit).ToString();
            spelling = spelling.Trim('\\', '\r', '\n');
            return spelling;
        });
    }

    public bool IsNegative => Handle.IsNegative;

    public bool IsNonNegative => Handle.IsNonNegative;

    public bool IsStrictlyPositive => Handle.IsStrictlyPositive;

    public long Value => Handle.IntegerLiteralValue;

    public string ValueString => _valueString.Value;
}
