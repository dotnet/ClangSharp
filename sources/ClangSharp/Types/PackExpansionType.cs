// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class PackExpansionType : Type
    {
        private readonly Lazy<Type> _desugaredType;

        internal PackExpansionType(CXType handle) : base(handle, CXTypeKind.CXType_Unexposed, CX_TypeClass.CX_TypeClass_PackExpansion)
        {
            _desugaredType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.Desugar()));
        }

        public bool IsSugared => Handle.IsSugared;

        public Type Desugar() => _desugaredType.Value;
    }
}
