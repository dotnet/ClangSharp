// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class FunctionType : Type
    {
        private readonly Lazy<Type> _returnType;

        private protected FunctionType(CXType handle, CXTypeKind expectedTypeKind, CX_TypeClass expectedTypeClass) : base(handle, expectedTypeKind, expectedTypeClass)
        {
            _returnType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.ResultType));
        }

        public CXCallingConv CallConv => Handle.FunctionTypeCallingConv;

        public Type ReturnType => _returnType.Value;
    }
}
