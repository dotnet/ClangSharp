// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public class TemplateDecl : NamedDecl
{
    private readonly LazyList<Expr> _associatedConstraints;
    private ValueLazy<TemplateDecl, NamedDecl> _templatedDecl;
    private readonly LazyList<NamedDecl> _templateParameters;

    private protected unsafe TemplateDecl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind, expectedDeclKind)
    {
        if (handle.DeclKind is > CX_DeclKind_LastTemplate or < CX_DeclKind_FirstTemplate)
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
        }

        _associatedConstraints = LazyList.Create<Expr>(this, Handle.NumAssociatedConstraints, &AssociatedConstraintsFactory);
        _templatedDecl = new ValueLazy<TemplateDecl, NamedDecl>(&TemplatedDeclFactory);
        _templateParameters = LazyList.Create<NamedDecl>(this, Handle.GetNumTemplateParameters(0), &TemplateParametersFactory);
    }

    public IReadOnlyList<Expr> AssociatedConstraints => _associatedConstraints;

    public bool HasAssociatedConstraints => AssociatedConstraints.Count != 0;

    public NamedDecl TemplatedDecl => _templatedDecl.GetValue(this);

    public IReadOnlyList<NamedDecl> TemplateParameters => _templateParameters;

    private static unsafe NamedDecl TemplatedDeclFactory(TemplateDecl self) => self.TranslationUnit.GetOrCreate<NamedDecl>(self.Handle.TemplatedDecl);

    private static unsafe Expr AssociatedConstraintsFactory(object self, int i)
    {
        var @this = (TemplateDecl)self;
        return @this.TranslationUnit.GetOrCreate<Expr>(@this.Handle.GetAssociatedConstraint(unchecked((uint)i)));
    }

    private static unsafe NamedDecl TemplateParametersFactory(object self, int i)
    {
        var @this = (TemplateDecl)self;
        return @this.TranslationUnit.GetOrCreate<NamedDecl>(@this.Handle.GetTemplateParameter(0, unchecked((uint)i)));
    }
}
