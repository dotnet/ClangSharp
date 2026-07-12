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
    private ValueLazy<TemplateTypeParmDecl, Type?> _defaultArgument;

    internal unsafe TemplateTypeParmDecl(CXCursor handle) : base(handle, CXCursor_TemplateTypeParameter, CX_DeclKind_TemplateTypeParm)
    {
        _associatedConstraints = LazyList.Create<Expr>(this, Handle.NumAssociatedConstraints, &AssociatedConstraintsFactory);
        _defaultArgument = new ValueLazy<TemplateTypeParmDecl, Type?>(&DefaultArgumentFactory);
    }

    public IReadOnlyList<Expr> AssociatedConstraints => _associatedConstraints;

    public Type? DefaultArgument => _defaultArgument.GetValue(this);

    public bool DefaultArgumentWasInherited => Handle.HasInheritedDefaultArg;

    public int Depth => Handle.TemplateTypeParmDepth;

    public int Index => Handle.TemplateTypeParmIndex;

    public bool HasDefaultArgument => Handle.HasDefaultArg;

    public bool IsExpandedParameterPack => Handle.IsExpandedParameterPack;

    public bool IsPackExpansion => Handle.IsPackExpansion;

    public bool IsParameterPack => Handle.IsParameterPack;

    private static unsafe Type? DefaultArgumentFactory(TemplateTypeParmDecl self) {
            var defaultArgType = self.Handle.DefaultArgType;
            return defaultArgType.kind == CXType_Invalid ? null : self.TranslationUnit.GetOrCreate<Type>(defaultArgType);
        }

    private static unsafe Expr AssociatedConstraintsFactory(object self, int i)
    {
        var @this = (TemplateTypeParmDecl)self;
        return @this.TranslationUnit.GetOrCreate<Expr>(@this.Handle.GetAssociatedConstraint(unchecked((uint)i)));
    }
}
