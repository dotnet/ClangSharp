// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class FunctionTemplateDecl : RedeclarableTemplateDecl
    {
        private readonly Lazy<IReadOnlyList<TemplateArgument>> _injectedTemplateArgs;

        internal FunctionTemplateDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_FunctionTemplate, CX_DeclKind.CX_DeclKind_FunctionTemplate)
        {
            _injectedTemplateArgs = new Lazy<IReadOnlyList<TemplateArgument>>(() => {
                var templateArgCount = Handle.NumTemplateArguments;
                var templateArgs = new List<TemplateArgument>(templateArgCount);

                for (int i = 0; i < templateArgCount; i++)
                {
                    var templateArg = TranslationUnit.GetOrCreate(Handle.GetTemplateArgument(unchecked((uint)i)));
                    templateArgs.Add(templateArg);
                }

                return templateArgs;
            });
        }

        public new FunctionTemplateDecl CanonicalDecl => (FunctionTemplateDecl)base.CanonicalDecl;

        public IReadOnlyList<TemplateArgument> InjectedTemplateArgs => _injectedTemplateArgs.Value;

        public new FunctionTemplateDecl InstantiatedFromMemberTemplate => (FunctionTemplateDecl)base.InstantiatedFromMemberTemplate;

        public bool IsThisDeclarationADefinition => Handle.IsThisDeclarationADefinition;

        public new FunctionTemplateDecl MostRecentDecl => (FunctionTemplateDecl)base.MostRecentDecl;

        public new FunctionTemplateDecl PreviousDecl => (FunctionTemplateDecl)base.PreviousDecl;

        public new FunctionDecl TemplatedDecl => (FunctionDecl)base.TemplatedDecl;
    }
}
