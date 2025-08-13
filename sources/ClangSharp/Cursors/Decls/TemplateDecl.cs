// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public class TemplateDecl : NamedDecl
{
    private readonly LazyList<Expr> _associatedConstraints;
    private readonly Lazy<NamedDecl> _templatedDecl;
    private readonly LazyList<NamedDecl> _templateParameters;

    private protected TemplateDecl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind, expectedDeclKind)
    {
        if (handle.DeclKind is > CX_DeclKind_LastTemplate or < CX_DeclKind_FirstTemplate)
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
        }

        _associatedConstraints = LazyList.Create<Expr>(Handle.NumAssociatedConstraints, (i) => TranslationUnit.GetOrCreate<Expr>(Handle.GetAssociatedConstraint(unchecked((uint)i))));
        _templatedDecl = new Lazy<NamedDecl>(() => TranslationUnit.GetOrCreate<NamedDecl>(Handle.TemplatedDecl));
        _templateParameters = LazyList.Create<NamedDecl>(Handle.GetNumTemplateParameters(0), (i) => TranslationUnit.GetOrCreate<NamedDecl>(Handle.GetTemplateParameter(0, unchecked((uint)i))));
    }

    public IReadOnlyList<Expr> AssociatedConstraints => _associatedConstraints;

    public bool HasAssociatedConstraints => AssociatedConstraints.Count != 0;

    public NamedDecl TemplatedDecl => _templatedDecl.Value;

    public IReadOnlyList<NamedDecl> TemplateParameters => _templateParameters;
}
