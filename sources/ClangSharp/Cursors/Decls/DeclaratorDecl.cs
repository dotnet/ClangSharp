// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public class DeclaratorDecl : ValueDecl
{
    private readonly LazyList<LazyList<NamedDecl>> _templateParameterLists;
    private readonly Lazy<Expr> _trailingRequiresClause;

    private protected DeclaratorDecl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind, expectedDeclKind)
    {
        if (handle.DeclKind is > CX_DeclKind_LastDeclarator or < CX_DeclKind_FirstDeclarator)
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
        }

        _templateParameterLists = LazyList.Create<LazyList<NamedDecl>>(Handle.NumTemplateParameterLists, (listIndex) => {
            var numTemplateParameters = Handle.GetNumTemplateParameters(unchecked((uint)listIndex));
            return LazyList.Create<NamedDecl>(numTemplateParameters, (parameterIndex) => TranslationUnit.GetOrCreate<NamedDecl>(Handle.GetTemplateParameter(unchecked((uint)listIndex), unchecked((uint)parameterIndex))));
        });
        _trailingRequiresClause = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.TrailingRequiresClause));
    }

    public uint NumTemplateParameterLists => unchecked((uint)Handle.NumTemplateParameterLists);

    public IReadOnlyList<IReadOnlyList<NamedDecl>> TemplateParameterLists => _templateParameterLists;

    public Expr TrailingRequiresClause => _trailingRequiresClause.Value;
}
