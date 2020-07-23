// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ElaboratedType : TypeWithKeyword
    {
        private readonly Lazy<Type> _desugaredType;
        private readonly Lazy<Type> _namedType;
        private readonly Lazy<TagDecl> _ownedTagDecl;

        internal ElaboratedType(CXType handle) : base(handle, CXTypeKind.CXType_Elaborated, CX_TypeClass.CX_TypeClass_Elaborated)
        {
            _desugaredType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.Desugar()));
            _namedType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.NamedType));
            _ownedTagDecl = new Lazy<TagDecl>(() => TranslationUnit.GetOrCreate<TagDecl>(Handle.OwnedTagDecl));
        }

        public bool IsSugared => Handle.IsSugared;

        public Type NamedType => _namedType.Value;

        public TagDecl OwnedTagDecl => _ownedTagDecl.Value;

        public Type Desugar() => _desugaredType.Value;
    }
}
