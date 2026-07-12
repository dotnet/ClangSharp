// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using System.Globalization;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_StmtClass;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CXTokenKind;

namespace ClangSharp;

public sealed class FloatingLiteral : Expr
{
    private ValueLazy<FloatingLiteral, string> _valueString;

    internal unsafe FloatingLiteral(CXCursor handle) : base(handle, CXCursor_FloatingLiteral, CX_StmtClass_FloatingLiteral)
    {
        _valueString = new ValueLazy<FloatingLiteral, string>(&ValueStringFactory);
    }

    public CX_FloatingSemantics RawSemantics => Handle.FloatingLiteralSemantics;

    public double ValueAsApproximateDouble => Handle.FloatingLiteralValueAsApproximateDouble;

    public string ValueString => _valueString.GetValue(this);

    private static unsafe string ValueStringFactory(FloatingLiteral self) {
            var tokens = self.Handle.TranslationUnit.Tokenize(self.Handle.SourceRange);

            if ((tokens.Length == 0) || (tokens[0].Kind is not CXToken_Literal and not CXToken_Identifier))
            {
                tokens = self.Handle.TranslationUnit.Tokenize(self.Handle.SourceRangeRaw);

                if ((tokens.Length == 0) || (tokens[0].Kind is not CXToken_Literal and not CXToken_Identifier))
                {
                    Debug.Assert(false, "Failed to stringify tokens for floating literal.");
                    return self.ValueAsApproximateDouble.ToString(CultureInfo.InvariantCulture);
                }
            }

            var spelling = tokens[0].GetSpelling(self.Handle.TranslationUnit).ToString();
            spelling = spelling.Trim('\\', '\r', '\n');
            return spelling;
        }
}
