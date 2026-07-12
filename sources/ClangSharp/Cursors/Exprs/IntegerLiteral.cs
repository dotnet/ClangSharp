// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using System.Globalization;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_StmtClass;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CXTokenKind;

namespace ClangSharp;

public sealed class IntegerLiteral : Expr
{
    private ValueLazy<IntegerLiteral, string> _valueString;

    internal unsafe IntegerLiteral(CXCursor handle) : base(handle, CXCursor_IntegerLiteral, CX_StmtClass_IntegerLiteral)
    {
        _valueString = new ValueLazy<IntegerLiteral, string>(&ValueStringFactory);
    }

    public bool IsNegative => Handle.IsNegative;

    public bool IsNonNegative => Handle.IsNonNegative;

    public bool IsStrictlyPositive => Handle.IsStrictlyPositive;

    public long Value => Handle.IntegerLiteralValue;

    public ulong UnsignedValue => Handle.UnsignedIntegerLiteralValue;

    public string ValueString => _valueString.GetValue(this);

    private static unsafe string ValueStringFactory(IntegerLiteral self) {
            var tokens = self.Handle.TranslationUnit.Tokenize(self.Handle.SourceRange);

            if ((tokens.Length == 0) || (tokens[0].Kind is not CXToken_Literal and not CXToken_Identifier))
            {
                tokens = self.Handle.TranslationUnit.Tokenize(self.Handle.SourceRangeRaw);

                if ((tokens.Length == 0) || (tokens[0].Kind is not CXToken_Literal and not CXToken_Identifier))
                {
                    Debug.Assert(false, "Failed to stringify tokens for integer literal.");
                    return self.Value.ToString(CultureInfo.InvariantCulture);
                }
            }

            var spelling = tokens[0].GetSpelling(self.Handle.TranslationUnit).ToString();
            spelling = spelling.Trim('\\', '\r', '\n');
            return spelling;
        }
}
