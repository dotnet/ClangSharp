// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class VarTemplateSpecializationDecl : VarDecl
    {
        private readonly Lazy<VarTemplateDecl> _specializedTemplate;
        private readonly Lazy<IReadOnlyList<TemplateArgument>> _templateArgs;

        internal VarTemplateSpecializationDecl(CXCursor handle) : this(handle, CXCursorKind.CXCursor_UnexposedDecl, CX_DeclKind.CX_DeclKind_VarTemplateSpecialization)
        {
        }

        private protected VarTemplateSpecializationDecl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind, expectedDeclKind)
        {
            if (handle.DeclKind is > CX_DeclKind.CX_DeclKind_LastVarTemplateSpecialization or < CX_DeclKind.CX_DeclKind_FirstVarTemplateSpecialization)
            {
                throw new ArgumentOutOfRangeException(nameof(handle));
            }

            _specializedTemplate = new Lazy<VarTemplateDecl>(() => TranslationUnit.GetOrCreate<VarTemplateDecl>(Handle.SpecializedCursorTemplate));
            _templateArgs = new Lazy<IReadOnlyList<TemplateArgument>>(() => {
                var templateArgCount = Handle.NumTemplateArguments;
                var templateArgs = new List<TemplateArgument>(templateArgCount);

                for (var i = 0; i < templateArgCount; i++)
                {
                    var templateArg = TranslationUnit.GetOrCreate(Handle.GetTemplateArgument(unchecked((uint)i)));
                    templateArgs.Add(templateArg);
                }

                return templateArgs;
            });
        }

        public new VarTemplateSpecializationDecl MostRecentDecl => (VarTemplateSpecializationDecl)base.MostRecentDecl;

        public VarTemplateDecl SpecializedTemplate => _specializedTemplate.Value;

        public IReadOnlyList<TemplateArgument> TemplateArgs => _templateArgs.Value;
    }
}
