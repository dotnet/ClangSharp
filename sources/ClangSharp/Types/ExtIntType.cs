// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ExtIntType : Type
    {
        private readonly Lazy<Type> _desugaredType;

        internal ExtIntType(CXType handle) : base(handle, CXTypeKind.CXType_Unexposed, CX_TypeClass.CX_TypeClass_ExtInt)
        {
            _desugaredType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.Desugar()));
        }

        public bool IsSigned => Handle.IsSigned;

        public bool IsSugared => Handle.IsSugared;

        public bool IsUnsigned => Handle.IsUnsigned;

        public uint NumBits => unchecked((uint)Handle.NumBits);

        public Type Desugar() => _desugaredType.Value;
    }
}
