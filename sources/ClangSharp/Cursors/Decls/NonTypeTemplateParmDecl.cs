// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class NonTypeTemplateParmDecl : DeclaratorDecl, ITemplateParmPosition
    {
        private readonly Lazy<IReadOnlyList<Expr>> _associatedConstraints;
        private readonly Lazy<Expr> _defaultArgument;
        private readonly Lazy<IReadOnlyList<Type>> _expansionTypes;
        private readonly Lazy<Expr> _placeholderTypeConstraint;

        internal NonTypeTemplateParmDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_NonTypeTemplateParameter, CX_DeclKind.CX_DeclKind_NonTypeTemplateParm)
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

            _defaultArgument = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.DefaultArg));

            _expansionTypes = new Lazy<IReadOnlyList<Type>>(() => {
                var numExpansionTypes = Handle.NumExpansionTypes;
                var expansionTypes = new List<Type>(numExpansionTypes);

                for (var i = 0; i < numExpansionTypes; i++)
                {
                    var expansionType = TranslationUnit.GetOrCreate<Type>(Handle.GetExpansionType(unchecked((uint)i)));
                    expansionTypes.Add(expansionType);
                }

                return expansionTypes;
            });

            _placeholderTypeConstraint = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.PlaceholderTypeConstraint));
        }

        public IReadOnlyList<Expr> AssociatedConstraints => _associatedConstraints.Value;

        public Expr DefaultArgument => _defaultArgument.Value;

        public bool DefaultArgumentWasInherited => Handle.HasInheritedDefaultArg;

        public uint Depth => unchecked((uint)Handle.TemplateTypeParmDepth);

        public IReadOnlyList<Type> ExpansionTypes => _expansionTypes.Value;

        public bool HasDefaultArgument => Handle.HasDefaultArg;

        public bool HasPlaceholderTypeConstraint => Handle.HasPlaceholderTypeConstraint;

        public uint Index => unchecked((uint)Handle.TemplateTypeParmIndex);

        public bool IsPackExpansion => Handle.IsPackExpansion;

        public bool IsParameterPack => Handle.IsParameterPack;

        public Expr PlaceholderTypeConstraint => _placeholderTypeConstraint.Value;

        public uint Position => unchecked((uint)Handle.TemplateTypeParmPosition);
    }
}
