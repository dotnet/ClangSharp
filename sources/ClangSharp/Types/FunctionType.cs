// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;

namespace ClangSharp;

public class FunctionType : Type
{
    private ValueLazy<FunctionType, Type> _returnType;

    private protected unsafe FunctionType(CXType handle, CXTypeKind expectedTypeKind, CX_TypeClass expectedTypeClass) : base(handle, expectedTypeKind, expectedTypeClass)
    {
        _returnType = new ValueLazy<FunctionType, Type>(&ReturnTypeFactory);
    }

    public CXCallingConv CallConv => Handle.FunctionTypeCallingConv;

    public Type ReturnType => _returnType.GetValue(this);

    private static unsafe Type ReturnTypeFactory(FunctionType self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.ResultType);
}
