// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class InjectedClassNameType : Type
    {
        private readonly Lazy<CXXRecordDecl> _decl;
        private readonly Lazy<Type> _injectedSpecializationType;
        private readonly Lazy<TemplateSpecializationType> _injectedTST;

        internal InjectedClassNameType(CXType handle) : base(handle, CXTypeKind.CXType_Unexposed, CX_TypeClass.CX_TypeClass_InjectedClassName)
        {
            _decl = new Lazy<CXXRecordDecl>(() => TranslationUnit.GetOrCreate<CXXRecordDecl>(handle.Declaration));
            _injectedSpecializationType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.InjectedSpecializationType));
            _injectedTST = new Lazy<TemplateSpecializationType>(() => TranslationUnit.GetOrCreate<TemplateSpecializationType>(Handle.InjectedTST));
        }

        public CXXRecordDecl Decl => _decl.Value;

        public Type InjectedSpecializationType => _injectedSpecializationType.Value;

        public TemplateSpecializationType InjectedTST => _injectedTST.Value;
    }
}
