// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_DeclKind;
using static ClangSharp.Interop.CXCursorKind;

namespace ClangSharp;

public class VarTemplatePartialSpecializationDecl : VarDecl
{
    private readonly LazyList<Expr> _associatedConstraints;
    private ValueLazy<VarTemplatePartialSpecializationDecl, VarTemplatePartialSpecializationDecl> _instantiatedFromMember;
    private readonly LazyList<NamedDecl> _templateParameters;

    internal unsafe VarTemplatePartialSpecializationDecl(CXCursor handle) : base(handle, CXCursor_UnexposedDecl, CX_DeclKind_VarTemplatePartialSpecialization)
    {
        _associatedConstraints = LazyList.Create<Expr>(this, Handle.NumAssociatedConstraints, &AssociatedConstraintsFactory);
        _instantiatedFromMember = new ValueLazy<VarTemplatePartialSpecializationDecl, VarTemplatePartialSpecializationDecl>(&InstantiatedFromMemberFactory);
        _templateParameters = LazyList.Create<NamedDecl>(this, Handle.GetNumTemplateParameters(0), &TemplateParametersFactory);
    }

    public IReadOnlyList<Expr> AssociatedConstraints => _associatedConstraints;

    public VarTemplatePartialSpecializationDecl InstantiatedFromMember => _instantiatedFromMember.GetValue(this);

    public new VarTemplatePartialSpecializationDecl MostRecentDecl => (VarTemplatePartialSpecializationDecl)base.MostRecentDecl;

    public IReadOnlyList<NamedDecl> TemplateParameters => _templateParameters;

    private static unsafe VarTemplatePartialSpecializationDecl InstantiatedFromMemberFactory(VarTemplatePartialSpecializationDecl self) => self.TranslationUnit.GetOrCreate<VarTemplatePartialSpecializationDecl>(self.Handle.InstantiatedFromMember);

    private static unsafe Expr AssociatedConstraintsFactory(object self, int i)
    {
        var @this = (VarTemplatePartialSpecializationDecl)self;
        return @this.TranslationUnit.GetOrCreate<Expr>(@this.Handle.GetAssociatedConstraint(unchecked((uint)i)));
    }

    private static unsafe NamedDecl TemplateParametersFactory(object self, int i)
    {
        var @this = (VarTemplatePartialSpecializationDecl)self;
        return @this.TranslationUnit.GetOrCreate<NamedDecl>(@this.Handle.GetTemplateParameter(0, unchecked((uint)i)));
    }
}
