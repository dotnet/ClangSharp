// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class TemplateSpecializationType : Type
    {
        private readonly Lazy<Type> _desugaredType;
        private readonly Lazy<IReadOnlyList<TemplateArgument>> _templateArgs;

        internal TemplateSpecializationType(CXType handle) : base(handle, CXTypeKind.CXType_Unexposed, CX_TypeClass.CX_TypeClass_TemplateSpecialization)
        {
            _desugaredType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.Desugar()));
            _templateArgs = new Lazy<IReadOnlyList<TemplateArgument>>(() => {
                var templateArgCount = Handle.NumTemplateArguments;
                var templateArgs = new List<TemplateArgument>(templateArgCount);

                for (int i = 0; i < templateArgCount; i++)
                {
                    var templateArg = new TemplateArgument(this, unchecked((uint)i));
                    templateArgs.Add(templateArg);
                }

                return templateArgs;
            });
        }

        public Type AliasedType => IsTypeAlias ? _desugaredType.Value : null;

        public IReadOnlyList<TemplateArgument> Args => _templateArgs.Value;

        public bool IsSugared => Handle.IsSugared;

        public bool IsTypeAlias => Handle.IsTypeAlias;

        public Type Desugar() => _desugaredType.Value;
    }
}
