// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class CompoundAssignOperator : BinaryOperator
{
    private ValueLazy<CompoundAssignOperator, Type> _computationLHSType;
    private ValueLazy<CompoundAssignOperator, Type> _computationResultType;

    internal unsafe CompoundAssignOperator(CXCursor handle) : base(handle, CXCursor_CompoundAssignOperator, CX_StmtClass_CompoundAssignOperator)
    {
        _computationLHSType = new ValueLazy<CompoundAssignOperator, Type>(&ComputationLHSTypeFactory);
        _computationResultType = new ValueLazy<CompoundAssignOperator, Type>(&ComputationResultTypeFactory);
    }

    public Type ComputationLHSType => _computationLHSType.GetValue(this);

    public Type ComputationResultType => _computationResultType.GetValue(this);

    private static unsafe Type ComputationResultTypeFactory(CompoundAssignOperator self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.ComputationResultType);

    private static unsafe Type ComputationLHSTypeFactory(CompoundAssignOperator self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.ComputationLhsType);
}
