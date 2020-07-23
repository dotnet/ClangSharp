// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class TemplateTypeParmType : Type
    {
        private readonly Lazy<TemplateTypeParmDecl> _decl;
        private readonly Lazy<Type> _desugaredType;

        internal TemplateTypeParmType(CXType handle) : base(handle, CXTypeKind.CXType_Unexposed, CX_TypeClass.CX_TypeClass_TemplateTypeParm)
        {
            _decl = new Lazy<TemplateTypeParmDecl>(() => TranslationUnit.GetOrCreate<TemplateTypeParmDecl>(Handle.Declaration));
            _desugaredType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.Desugar()));
        }

        public TemplateTypeParmDecl Decl => _decl.Value;

        public uint Depth => unchecked((uint)Handle.Depth);

        public uint Index => unchecked((uint)Handle.Index);

        public bool IsSugared => Handle.IsSugared;

        public Type Desugar() => _desugaredType.Value;
    }
}
