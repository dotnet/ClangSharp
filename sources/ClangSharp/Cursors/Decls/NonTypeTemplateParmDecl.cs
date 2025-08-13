// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_DeclKind;
using static ClangSharp.Interop.CXCursorKind;

namespace ClangSharp;

public sealed class NonTypeTemplateParmDecl : DeclaratorDecl, ITemplateParmPosition
{
    private readonly LazyList<Expr> _associatedConstraints;
    private readonly Lazy<Expr> _defaultArgument;
    private readonly LazyList<Type> _expansionTypes;
    private readonly Lazy<Expr> _placeholderTypeConstraint;

    internal NonTypeTemplateParmDecl(CXCursor handle) : base(handle, CXCursor_NonTypeTemplateParameter, CX_DeclKind_NonTypeTemplateParm)
    {
        _associatedConstraints = LazyList.Create<Expr>(Handle.NumAssociatedConstraints, (i) => TranslationUnit.GetOrCreate<Expr>(Handle.GetAssociatedConstraint(unchecked((uint)i))));
        _defaultArgument = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.DefaultArg));
        _expansionTypes = LazyList.Create<Type>(Handle.NumExpansionTypes, (i) => TranslationUnit.GetOrCreate<Type>(Handle.GetExpansionType(unchecked((uint)i))));
        _placeholderTypeConstraint = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.PlaceholderTypeConstraint));
    }

    public IReadOnlyList<Expr> AssociatedConstraints => _associatedConstraints;

    public Expr DefaultArgument => _defaultArgument.Value;

    public bool DefaultArgumentWasInherited => Handle.HasInheritedDefaultArg;

    public uint Depth => unchecked((uint)Handle.TemplateTypeParmDepth);

    public IReadOnlyList<Type> ExpansionTypes => _expansionTypes;

    public bool HasDefaultArgument => Handle.HasDefaultArg;

    public bool HasPlaceholderTypeConstraint => Handle.HasPlaceholderTypeConstraint;

    public uint Index => unchecked((uint)Handle.TemplateTypeParmIndex);

    public bool IsPackExpansion => Handle.IsPackExpansion;

    public bool IsParameterPack => Handle.IsParameterPack;

    public Expr PlaceholderTypeConstraint => _placeholderTypeConstraint.Value;

    public uint Position => unchecked((uint)Handle.TemplateTypeParmPosition);
}
