// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class DeclaratorDecl : ValueDecl
    {
        private readonly Lazy<IReadOnlyList<IReadOnlyList<NamedDecl>>> _templateParameterLists;
        private readonly Lazy<Expr> _trailingRequiresClause;

        private protected DeclaratorDecl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind, expectedDeclKind)
        {
            if ((CX_DeclKind.CX_DeclKind_LastDeclarator < handle.DeclKind) || (handle.DeclKind < CX_DeclKind.CX_DeclKind_FirstDeclarator))
            {
                throw new ArgumentException(nameof(handle));
            }

            _templateParameterLists = new Lazy<IReadOnlyList<IReadOnlyList<NamedDecl>>>(() => {
                var numTemplateParameterLists = Handle.NumTemplateParameterLists;
                var templateParameterLists = new List<IReadOnlyList<NamedDecl>>(numTemplateParameterLists);

                for (var listIndex = 0; listIndex < numTemplateParameterLists; listIndex++)
                {
                    var numTemplateParameters = Handle.GetNumTemplateParameters(unchecked((uint)listIndex));
                    var templateParameterList = new List<NamedDecl>(numTemplateParameters);

                    for (var parameterIndex = 0; parameterIndex < numTemplateParameters; parameterIndex++)
                    {
                        var templateParameter = TranslationUnit.GetOrCreate<NamedDecl>(Handle.GetTemplateParameter(unchecked((uint)listIndex), unchecked((uint)parameterIndex)));
                        templateParameterList.Add(templateParameter);
                    }

                    templateParameterLists.Add(templateParameterList);
                }

                return templateParameterLists;
            });

            _trailingRequiresClause = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.TrailingRequiresClause));
        }

        public uint NumTemplateParameterLists => unchecked((uint)Handle.NumTemplateParameterLists);

        public IReadOnlyList<IReadOnlyList<NamedDecl>> TemplateParameterLists => _templateParameterLists.Value;

        public Expr TrailingRequiresClause => _trailingRequiresClause.Value;
    }
}
