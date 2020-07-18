// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class UnresolvedUsingType : Type
    {
        private readonly Lazy<UnresolvedUsingTypenameDecl> _decl;
        private readonly Lazy<Type> _desugaredType;

        internal UnresolvedUsingType(CXType handle) : base(handle, CXTypeKind.CXType_Unexposed, CX_TypeClass.CX_TypeClass_UnresolvedUsing)
        {
            _decl = new Lazy<UnresolvedUsingTypenameDecl>(() => TranslationUnit.GetOrCreate<UnresolvedUsingTypenameDecl>(Handle.Declaration));
            _desugaredType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.Desugar()));
        }

        public UnresolvedUsingTypenameDecl Decl => _decl.Value;

        public bool IsSugared => Handle.IsSugared;

        public Type Desugar() => _desugaredType.Value;
    }
}
