// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class TypedefType : Type
    {
        private readonly Lazy<TypedefNameDecl> _decl;
        private readonly Lazy<Type> _desugaredType;

        internal TypedefType(CXType handle) : base(handle, CXTypeKind.CXType_Typedef, CX_TypeClass.CX_TypeClass_Typedef)
        {
            _decl = new Lazy<TypedefNameDecl>(() => TranslationUnit.GetOrCreate<TypedefNameDecl>(Handle.Declaration));
            _desugaredType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.Desugar()));
        }

        public bool IsSugared => Handle.IsSugared;

        public Type Desugar() => _desugaredType.Value;

        public TypedefNameDecl Decl => _decl.Value;
    }
}
