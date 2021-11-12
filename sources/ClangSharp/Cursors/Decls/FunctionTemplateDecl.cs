// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class FunctionTemplateDecl : RedeclarableTemplateDecl
    {
        private readonly Lazy<IReadOnlyList<TemplateArgument>> _injectedTemplateArgs;
        private readonly Lazy<IReadOnlyList<FunctionDecl>> _specializations;

        internal FunctionTemplateDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_FunctionTemplate, CX_DeclKind.CX_DeclKind_FunctionTemplate)
        {
            _injectedTemplateArgs = new Lazy<IReadOnlyList<TemplateArgument>>(() => {
                var templateArgCount = Handle.NumTemplateArguments;
                var templateArgs = new List<TemplateArgument>(templateArgCount);

                for (var i = 0; i < templateArgCount; i++)
                {
                    var templateArg = TranslationUnit.GetOrCreate(Handle.GetTemplateArgument(unchecked((uint)i)));
                    templateArgs.Add(templateArg);
                }

                return templateArgs;
            });

            _specializations = new Lazy<IReadOnlyList<FunctionDecl>>(() => {
                var numSpecializations = Handle.NumSpecializations;
                var specializations = new List<FunctionDecl>(numSpecializations);

                for (var i = 0; i < numSpecializations; i++)
                {
                    var specialization = TranslationUnit.GetOrCreate<FunctionDecl>(Handle.GetSpecialization(unchecked((uint)i)));
                    specializations.Add(specialization);
                }

                return specializations;
            });
        }

        public new FunctionTemplateDecl CanonicalDecl => (FunctionTemplateDecl)base.CanonicalDecl;

        public IReadOnlyList<TemplateArgument> InjectedTemplateArgs => _injectedTemplateArgs.Value;

        public new FunctionTemplateDecl InstantiatedFromMemberTemplate => (FunctionTemplateDecl)base.InstantiatedFromMemberTemplate;

        public bool IsThisDeclarationADefinition => Handle.IsThisDeclarationADefinition;

        public new FunctionTemplateDecl MostRecentDecl => (FunctionTemplateDecl)base.MostRecentDecl;

        public new FunctionTemplateDecl PreviousDecl => (FunctionTemplateDecl)base.PreviousDecl;

        public IReadOnlyList<FunctionDecl> Specializations => _specializations.Value;

        public new FunctionDecl TemplatedDecl => (FunctionDecl)base.TemplatedDecl;
    }
}
