// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using ClangSharp.Interop;
using System;
using System.Collections.Generic;

namespace ClangSharp
{
    public class TemplateDecl : NamedDecl
    {
        private readonly Lazy<IReadOnlyList<Expr>> _associatedConstraints;
        private readonly Lazy<NamedDecl> _templatedDecl;
        private readonly Lazy<IReadOnlyList<NamedDecl>> _templateParameters;

        private protected TemplateDecl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind, expectedDeclKind)
        {
            if ((CX_DeclKind.CX_DeclKind_LastTemplate < handle.DeclKind) || (handle.DeclKind < CX_DeclKind.CX_DeclKind_FirstTemplate))
            {
                throw new ArgumentException(nameof(handle));
            }

            _associatedConstraints = new Lazy<IReadOnlyList<Expr>>(() => {
                var associatedConstraintCount = Handle.NumAssociatedConstraints;
                var associatedConstraints = new List<Expr>(associatedConstraintCount);

                for (int i = 0; i < associatedConstraintCount; i++)
                {
                    var parameter = TranslationUnit.GetOrCreate<Expr>(Handle.GetAssociatedConstraint(unchecked((uint)i)));
                    associatedConstraints.Add(parameter);
                }

                return associatedConstraints;
            });
            _templatedDecl = new Lazy<NamedDecl>(() => TranslationUnit.GetOrCreate<CXXMethodDecl>(Handle.TemplatedDecl));
            _templateParameters = new Lazy<IReadOnlyList<NamedDecl>>(() => {
                var parameterCount = Handle.NumTemplateArguments;
                var parameters = new List<NamedDecl>(parameterCount);

                for (int i = 0; i < parameterCount; i++)
                {
                    var parameter = TranslationUnit.GetOrCreate<NamedDecl>(Handle.GetTemplateParameter(unchecked((uint)i)));
                    parameters.Add(parameter);
                }

                return parameters;
            });
        }

        public IReadOnlyList<Expr> AssociatedConstraints => _associatedConstraints.Value;

        public bool HasAssociatedConstraints => AssociatedConstraints.Count != 0;

        public NamedDecl TemplatedDecl => _templatedDecl.Value;

        public IReadOnlyList<NamedDecl> TemplateParameters => _templateParameters.Value;
    }
}
