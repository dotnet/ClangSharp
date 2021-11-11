// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

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
