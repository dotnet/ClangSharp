// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_DeclKind;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CXTypeKind;

namespace ClangSharp;

public sealed class TemplateTypeParmDecl : TypeDecl
{
    private readonly LazyList<Expr> _associatedConstraints;
    private readonly ValueLazy<Type?> _defaultArgument;

    internal TemplateTypeParmDecl(CXCursor handle) : base(handle, CXCursor_TemplateTypeParameter, CX_DeclKind_TemplateTypeParm)
    {
        _associatedConstraints = LazyList.Create<Expr>(Handle.NumAssociatedConstraints, (i) => TranslationUnit.GetOrCreate<Expr>(Handle.GetAssociatedConstraint(unchecked((uint)i))));
        _defaultArgument = new ValueLazy<Type?>(() => {
            var defaultArgType = Handle.DefaultArgType;
            return defaultArgType.kind == CXType_Invalid ? null : TranslationUnit.GetOrCreate<Type>(defaultArgType);
        });
    }

    public IReadOnlyList<Expr> AssociatedConstraints => _associatedConstraints;

    public Type? DefaultArgument => _defaultArgument.Value;

    public bool DefaultArgumentWasInherited => Handle.HasInheritedDefaultArg;

    public int Depth => Handle.TemplateTypeParmDepth;

    public int Index => Handle.TemplateTypeParmIndex;

    public bool HasDefaultArgument => Handle.HasDefaultArg;

    public bool IsExpandedParameterPack => Handle.IsExpandedParameterPack;

    public bool IsPackExpansion => Handle.IsPackExpansion;

    public bool IsParameterPack => Handle.IsParameterPack;
}
