// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class CXXTemporaryObjectExpr : CXXConstructExpr
{
    private ValueLazy<CXXTemporaryObjectExpr, Type> _typeSourceInfoType;

    internal unsafe CXXTemporaryObjectExpr(CXCursor handle) : base(handle, CXCursor_CallExpr, CX_StmtClass_CXXTemporaryObjectExpr)
    {
        _typeSourceInfoType = new ValueLazy<CXXTemporaryObjectExpr, Type>(&TypeSourceInfoTypeFactory);
    }

    public Type TypeSourceInfoType => _typeSourceInfoType.GetValue(this);

    private static unsafe Type TypeSourceInfoTypeFactory(CXXTemporaryObjectExpr self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.TypeOperand);
}
