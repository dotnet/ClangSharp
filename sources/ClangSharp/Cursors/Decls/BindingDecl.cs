// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public sealed class BindingDecl : ValueDecl
{
    private readonly ValueLazy<Expr> _binding;
    private readonly ValueLazy<ValueDecl> _decomposedDecl;
    private readonly ValueLazy<VarDecl> _holdingVar;

    internal BindingDecl(CXCursor handle) : base(handle, CXCursor_UnexposedDecl, CX_DeclKind_Binding)
    {
        _binding = new ValueLazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.BindingExpr));
        _decomposedDecl = new ValueLazy<ValueDecl>(() => TranslationUnit.GetOrCreate<ValueDecl>(Handle.DecomposedDecl));
        _holdingVar = new ValueLazy<VarDecl>(() => TranslationUnit.GetOrCreate<VarDecl>(Handle.GetSubDecl(0)));
    }

    public Expr Binding => _binding.Value;

    public ValueDecl DecomposedDecl => _decomposedDecl.Value;

    public VarDecl HoldingVar => _holdingVar.Value;
}
