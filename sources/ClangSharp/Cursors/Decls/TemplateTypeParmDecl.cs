// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public sealed class TemplateTypeParmDecl : TypeDecl
{
    private readonly Lazy<IReadOnlyList<Expr>> _associatedConstraints;
    private readonly Lazy<Type> _defaultArgument;

    internal TemplateTypeParmDecl(CXCursor handle) : base(handle, CXCursor_TemplateTypeParameter, CX_DeclKind_TemplateTypeParm)
    {
        _associatedConstraints = new Lazy<IReadOnlyList<Expr>>(() => {
            var associatedConstraintCount = Handle.NumAssociatedConstraints;
            var associatedConstraints = new List<Expr>(associatedConstraintCount);

            for (var i = 0; i < associatedConstraintCount; i++)
            {
                var parameter = TranslationUnit.GetOrCreate<Expr>(Handle.GetAssociatedConstraint(unchecked((uint)i)));
                associatedConstraints.Add(parameter);
            }

            return associatedConstraints;
        });
        _defaultArgument = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.DefaultArgType));
    }

    public IReadOnlyList<Expr> AssociatedConstraints => _associatedConstraints.Value;

    public Type DefaultArgument => _defaultArgument.Value;

    public bool DefaultArgumentWasInherited => Handle.HasInheritedDefaultArg;

    public int Depth => Handle.TemplateTypeParmDepth;

    public int Index => Handle.TemplateTypeParmIndex;

    public bool HasDefaultArgument => Handle.HasDefaultArg;

    public bool IsExpandedParameterPack => Handle.IsExpandedParameterPack;

    public bool IsPackExpansion => Handle.IsPackExpansion;

    public bool IsParameterPack => Handle.IsParameterPack;
}
