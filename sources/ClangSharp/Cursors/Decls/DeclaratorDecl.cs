// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public class DeclaratorDecl : ValueDecl
{
    private readonly Lazy<IReadOnlyList<IReadOnlyList<NamedDecl>>> _templateParameterLists;
    private readonly Lazy<Expr> _trailingRequiresClause;

    private protected DeclaratorDecl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind, expectedDeclKind)
    {
        if (handle.DeclKind is > CX_DeclKind_LastDeclarator or < CX_DeclKind_FirstDeclarator)
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
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
