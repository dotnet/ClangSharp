// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;

namespace ClangSharp;

public sealed class CXXTemporaryObjectExpr : CXXConstructExpr
{
    private readonly Lazy<Type> _typeSourceInfoType;

    internal CXXTemporaryObjectExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_CallExpr, CX_StmtClass.CX_StmtClass_CXXTemporaryObjectExpr)
    {
        _typeSourceInfoType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.TypeOperand));
    }

    public Type TypeSourceInfoType => _typeSourceInfoType.Value;
}
