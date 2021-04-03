// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class VarTemplatePartialSpecializationDecl : VarDecl
    {
        private readonly Lazy<IReadOnlyList<Expr>> _associatedConstraints;
        private readonly Lazy<VarTemplatePartialSpecializationDecl> _instantiatedFromMember;
        private readonly Lazy<IReadOnlyList<NamedDecl>> _templateParameters;

        internal VarTemplatePartialSpecializationDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnexposedDecl, CX_DeclKind.CX_DeclKind_VarTemplatePartialSpecialization)
        {
            _associatedConstraints = new Lazy<IReadOnlyList<Expr>>(() => {
                var associatedConstraintCount = Handle.NumAssociatedConstraints;
                var associatedConstraints = new List<Expr>(associatedConstraintCount);

                for (var i = 0; i < associatedConstraintCount; i++)
                {
                    var parameter = TranslationUnit.GetOrCreate<Expr>(Handle.GetAssociatedConstraint(unchecked((uint)i)));
                    associatedConstraints.Add(parameter);
                }

                return associatedConstraints;
            });
            _instantiatedFromMember = new Lazy<VarTemplatePartialSpecializationDecl>(() => TranslationUnit.GetOrCreate<VarTemplatePartialSpecializationDecl>(Handle.InstantiatedFromMember));
            _templateParameters = new Lazy<IReadOnlyList<NamedDecl>>(() => {
                var parameterCount = Handle.GetNumTemplateParameters(0);
                var parameters = new List<NamedDecl>(parameterCount);

                for (var i = 0; i < parameterCount; i++)
                {
                    var parameter = TranslationUnit.GetOrCreate<NamedDecl>(Handle.GetTemplateParameter(0, unchecked((uint)i)));
                    parameters.Add(parameter);
                }

                return parameters;
            });
        }

        public IReadOnlyList<Expr> AssociatedConstraints => _associatedConstraints.Value;

        public VarTemplatePartialSpecializationDecl InstantiatedFromMember => _instantiatedFromMember.Value;

        public new VarTemplatePartialSpecializationDecl MostRecentDecl => (VarTemplatePartialSpecializationDecl)base.MostRecentDecl;

        public IReadOnlyList<NamedDecl> TemplateParameters => _templateParameters.Value;
    }
}
