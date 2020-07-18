// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class FunctionNoProtoType : FunctionType
    {
        private readonly Lazy<Type> _desugaredType;

        internal FunctionNoProtoType(CXType handle) : base(handle, CXTypeKind.CXType_FunctionNoProto, CX_TypeClass.CX_TypeClass_FunctionNoProto)
        {
            _desugaredType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.Desugar()));
        }

        public bool IsSugared => Handle.IsSugared;

        public Type Desugar() => _desugaredType.Value;
    }
}
