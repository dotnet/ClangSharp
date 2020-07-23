// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class BlockPointerType : Type
    {
        private readonly Lazy<Type> _desugaredType;
        private readonly Lazy<Type> _pointeeType;

        internal BlockPointerType(CXType handle) : base(handle, CXTypeKind.CXType_BlockPointer, CX_TypeClass.CX_TypeClass_BlockPointer)
        {
            _desugaredType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.Desugar()));
            _pointeeType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.PointeeType));
        }

        public bool IsSugared => Handle.IsSugared;

        public Type PointeeType => _pointeeType.Value;

        public Type Desugar() => _desugaredType.Value;
    }
}
