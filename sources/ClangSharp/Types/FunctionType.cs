// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class FunctionType : Type
    {
        private readonly Lazy<Type> _returnType;

        private protected FunctionType(CXType handle, CXTypeKind expectedKind) : base(handle, expectedKind)
        {
            _returnType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.ResultType));
        }

        public CXCallingConv CallConv => Handle.FunctionTypeCallingConv;

        public Type ReturnType => _returnType.Value;
    }
}
