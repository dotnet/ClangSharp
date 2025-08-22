// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class CXXScalarValueInitExpr : Expr
{
    private readonly ValueLazy<Type> _typeSourceInfoType;

    internal CXXScalarValueInitExpr(CXCursor handle) : base(handle, CXCursor_UnexposedExpr, CX_StmtClass_CXXScalarValueInitExpr)
    {
        Debug.Assert(NumChildren is 0);
        _typeSourceInfoType = new ValueLazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.TypeOperand));
    }

    public Type TypeSourceInfoType => _typeSourceInfoType.Value;
}
