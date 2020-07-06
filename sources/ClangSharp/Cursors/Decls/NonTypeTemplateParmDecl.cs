// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class NonTypeTemplateParmDecl : DeclaratorDecl, ITemplateParmPosition
    {
        private readonly Lazy<IReadOnlyList<Expr>> _associatedConstraints;
        private readonly Lazy<Expr> _defaultArgument;
        private readonly Lazy<Expr> _placeholderTypeConstraint;

        internal NonTypeTemplateParmDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_NonTypeTemplateParameter, CX_DeclKind.CX_DeclKind_NonTypeTemplateParm)
        {
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
            _defaultArgument = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.DefaultArg));
            _placeholderTypeConstraint = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.PlaceholderTypeConstraint));
        }

        public IReadOnlyList<Expr> AssociatedConstraints => _associatedConstraints.Value;

        public Expr DefaultArgument => _defaultArgument.Value;

        public bool DefaultArgumentWasInherited => Handle.HasInheritedDefaultArg;

        public bool HasDefaultArgument => Handle.HasDefaultArg;

        public bool HasPlaceholderTypeConstraint => Handle.HasPlaceholderTypeConstraint;

        public bool IsPackExpansion => Handle.IsPackExpansion;

        public bool IsParameterPack => Handle.IsParameterPack;

        public Expr PlaceholderTypeConstraint => _placeholderTypeConstraint.Value;
    }
}
