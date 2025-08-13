// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Diagnostics;
using System.Globalization;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_StmtClass;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CXTokenKind;

namespace ClangSharp;

public sealed class CharacterLiteral : Expr
{
    private readonly ValueLazy<string> _valueString;

    internal CharacterLiteral(CXCursor handle) : base(handle, CXCursor_CharacterLiteral, CX_StmtClass_CharacterLiteral)
    {
        Debug.Assert(NumChildren is 0);

        _valueString = new ValueLazy<string>(() => {
            var tokens = Handle.TranslationUnit.Tokenize(Handle.SourceRange);

            if ((tokens.Length == 0) || (tokens[0].Kind is not CXToken_Literal and not CXToken_Identifier))
            {
                tokens = Handle.TranslationUnit.Tokenize(Handle.SourceRangeRaw);

                if ((tokens.Length == 0) || (tokens[0].Kind is not CXToken_Literal and not CXToken_Identifier))
                {
                    Debug.Assert(false, "Failed to stringify tokens for character literal.");
                    return Value.ToString(CultureInfo.InvariantCulture);
                }
            }

            var spelling = tokens[0].GetSpelling(Handle.TranslationUnit).ToString();
            spelling = spelling.Trim('\\', '\r', '\n');
            return spelling;
        });
    }

    public CX_CharacterKind Kind => Handle.CharacterLiteralKind;

    public uint Value => Handle.CharacterLiteralValue;

    public string ValueString => _valueString.Value;
}
