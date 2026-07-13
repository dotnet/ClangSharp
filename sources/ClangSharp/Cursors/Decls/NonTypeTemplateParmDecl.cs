// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_DeclKind;
using static ClangSharp.Interop.CXCursorKind;

namespace ClangSharp;

public sealed class NonTypeTemplateParmDecl : DeclaratorDecl, ITemplateParmPosition
{
    private readonly LazyList<Expr> _associatedConstraints;
    private ValueLazy<NonTypeTemplateParmDecl, Expr> _defaultArgument;
    private readonly LazyList<Type> _expansionTypes;
    private ValueLazy<NonTypeTemplateParmDecl, Expr> _placeholderTypeConstraint;

    internal unsafe NonTypeTemplateParmDecl(CXCursor handle) : base(handle, CXCursor_NonTypeTemplateParameter, CX_DeclKind_NonTypeTemplateParm)
    {
        _associatedConstraints = LazyList.Create<Expr>(this, Handle.NumAssociatedConstraints, &AssociatedConstraintsFactory);
        _defaultArgument = new ValueLazy<NonTypeTemplateParmDecl, Expr>(&DefaultArgumentFactory);
        _expansionTypes = LazyList.Create<Type>(this, Handle.NumExpansionTypes, &ExpansionTypesFactory);
        _placeholderTypeConstraint = new ValueLazy<NonTypeTemplateParmDecl, Expr>(&PlaceholderTypeConstraintFactory);
    }

    public IReadOnlyList<Expr> AssociatedConstraints => _associatedConstraints;

    public Expr DefaultArgument => _defaultArgument.GetValue(this);

    public bool DefaultArgumentWasInherited => Handle.HasInheritedDefaultArg;

    public uint Depth => unchecked((uint)Handle.TemplateTypeParmDepth);

    public IReadOnlyList<Type> ExpansionTypes => _expansionTypes;

    public bool HasDefaultArgument => Handle.HasDefaultArg;

    public bool HasPlaceholderTypeConstraint => Handle.HasPlaceholderTypeConstraint;

    public uint Index => unchecked((uint)Handle.TemplateTypeParmIndex);

    public bool IsPackExpansion => Handle.IsPackExpansion;

    public bool IsParameterPack => Handle.IsParameterPack;

    public Expr PlaceholderTypeConstraint => _placeholderTypeConstraint.GetValue(this);

    public uint Position => unchecked((uint)Handle.TemplateTypeParmPosition);

    private static unsafe Expr PlaceholderTypeConstraintFactory(NonTypeTemplateParmDecl self) => self.TranslationUnit.GetOrCreate<Expr>(self.Handle.PlaceholderTypeConstraint);

    private static unsafe Expr DefaultArgumentFactory(NonTypeTemplateParmDecl self) => self.TranslationUnit.GetOrCreate<Expr>(self.Handle.DefaultArg);

    private static unsafe Expr AssociatedConstraintsFactory(object self, int i)
    {
        var @this = (NonTypeTemplateParmDecl)self;
        return @this.TranslationUnit.GetOrCreate<Expr>(@this.Handle.GetAssociatedConstraint(unchecked((uint)i)));
    }

    private static unsafe Type ExpansionTypesFactory(object self, int i)
    {
        var @this = (NonTypeTemplateParmDecl)self;
        return @this.TranslationUnit.GetOrCreate<Type>(@this.Handle.GetExpansionType(unchecked((uint)i)));
    }
}
