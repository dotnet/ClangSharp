// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public class ExplicitCastExpr : CastExpr
{
    private readonly ValueLazy<Type> _typeAsWritten;

    private protected ExplicitCastExpr(CXCursor handle, CXCursorKind expectedCursorKind, CX_StmtClass expectedStmtClass) : base(handle, expectedCursorKind, expectedStmtClass)
    {
        if (handle.StmtClass is > CX_StmtClass_LastExplicitCastExpr or < CX_StmtClass_FirstExplicitCastExpr)
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
        }

        _typeAsWritten = new ValueLazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.TypeOperand));
    }

    public Type TypeAsWritten => _typeAsWritten.Value;
}
