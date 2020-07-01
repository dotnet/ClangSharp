// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ClassScopeFunctionSpecializationDecl : Decl
    {
        private readonly Lazy<CXXMethodDecl> _specialization;
        private readonly Lazy<IReadOnlyList<TemplateArgumentLoc>> _templateArgs;

        internal ClassScopeFunctionSpecializationDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnexposedDecl, CX_DeclKind.CX_DeclKind_ClassScopeFunctionSpecialization)
        {
            _specialization = new Lazy<CXXMethodDecl>(() => TranslationUnit.GetOrCreate<CXXMethodDecl>(Handle.GetSpecialization(0)));
            _templateArgs = new Lazy<IReadOnlyList<TemplateArgumentLoc>>(() => {
                var templateArgCount = Handle.NumTemplateArguments;
                var templateArgs = new List<TemplateArgumentLoc>(templateArgCount);

                for (int i = 0; i < templateArgCount; i++)
                {
                    var templateArg = new TemplateArgumentLoc(this, unchecked((uint)i));
                    templateArgs.Add(templateArg);
                }

                return templateArgs;
            });
        }

        public bool HasExplicitTemplateArgs => Handle.HasExplicitTemplateArgs;

        public CXXMethodDecl Specialization => _specialization.Value;

        public IReadOnlyList<TemplateArgumentLoc> TemplateArgs => _templateArgs.Value;
    }
}
