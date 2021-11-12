// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

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

                for (var i = 0; i < templateArgCount; i++)
                {
                    var templateArg = TranslationUnit.GetOrCreate(Handle.GetTemplateArgumentLoc(unchecked((uint)i)));
                    templateArgs.Add(templateArg);
                }

                return templateArgs;
            });
        }

        public bool HasExplicitTemplateArgs => Handle.HasExplicitTemplateArgs;

        public uint NumTemplateArgs => unchecked((uint)Handle.NumTemplateArguments);

        public CXXMethodDecl Specialization => _specialization.Value;

        public IReadOnlyList<TemplateArgumentLoc> TemplateArgs => _templateArgs.Value;
    }
}
