// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class SubstTemplateTypeParmType : Type
    {
        private readonly Lazy<TemplateTypeParmType> _replacedParameter;

        internal SubstTemplateTypeParmType(CXType handle) : base(handle, CXTypeKind.CXType_Unexposed, CX_TypeClass.CX_TypeClass_SubstTemplateTypeParm)
        {
            _replacedParameter = new Lazy<TemplateTypeParmType>(() => TranslationUnit.GetOrCreate<TemplateTypeParmType>(Handle.OriginalType));
        }

        public TemplateTypeParmType ReplacedParameter => _replacedParameter.Value;

        public Type ReplacementType => Desugar;
    }
}
