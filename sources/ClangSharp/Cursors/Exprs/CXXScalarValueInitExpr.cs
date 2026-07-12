// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class CXXScalarValueInitExpr : Expr
{
    private ValueLazy<CXXScalarValueInitExpr, Type> _typeSourceInfoType;

    internal unsafe CXXScalarValueInitExpr(CXCursor handle) : base(handle, CXCursor_UnexposedExpr, CX_StmtClass_CXXScalarValueInitExpr)
    {
        Debug.Assert(NumChildren is 0);
        _typeSourceInfoType = new ValueLazy<CXXScalarValueInitExpr, Type>(&TypeSourceInfoTypeFactory);
    }

    public Type TypeSourceInfoType => _typeSourceInfoType.GetValue(this);

    private static unsafe Type TypeSourceInfoTypeFactory(CXXScalarValueInitExpr self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.TypeOperand);
}
