// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class CompoundAssignOperator : BinaryOperator
{
    private readonly ValueLazy<Type> _computationLHSType;
    private readonly ValueLazy<Type> _computationResultType;

    internal CompoundAssignOperator(CXCursor handle) : base(handle, CXCursor_CompoundAssignOperator, CX_StmtClass_CompoundAssignOperator)
    {
        _computationLHSType = new ValueLazy<Type>(() => TranslationUnit.GetOrCreate<Type>(handle.ComputationLhsType));
        _computationResultType = new ValueLazy<Type>(() => TranslationUnit.GetOrCreate<Type>(handle.ComputationResultType));
    }

    public Type ComputationLHSType => _computationLHSType.Value;

    public Type ComputationResultType => _computationResultType.Value;
}
