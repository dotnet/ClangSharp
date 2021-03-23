// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class DeducedTemplateSpecializationType : DeducedType
    {
        private readonly Lazy<TemplateName> _templateName;

        internal DeducedTemplateSpecializationType(CXType handle) : base(handle, CXTypeKind.CXType_Unexposed, CX_TypeClass.CX_TypeClass_DeducedTemplateSpecialization)
        {
            _templateName = new Lazy<TemplateName>(() => TranslationUnit.GetOrCreate(Handle.TemplateName));
        }

        public TemplateName TemplateName => _templateName.Value;
    }
}
