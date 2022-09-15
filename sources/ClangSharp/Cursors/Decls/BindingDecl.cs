// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;

namespace ClangSharp;

public sealed class BindingDecl : ValueDecl
{
    private readonly Lazy<Expr> _binding;
    private readonly Lazy<ValueDecl> _decomposedDecl;
    private readonly Lazy<VarDecl> _holdingVar;

    internal BindingDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnexposedDecl, CX_DeclKind.CX_DeclKind_Binding)
    {
        _binding = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.BindingExpr));
        _decomposedDecl = new Lazy<ValueDecl>(() => TranslationUnit.GetOrCreate<ValueDecl>(Handle.DecomposedDecl));
        _holdingVar = new Lazy<VarDecl>(() => TranslationUnit.GetOrCreate<VarDecl>(Handle.GetSubDecl(0)));
    }

    public Expr Binding => _binding.Value;

    public ValueDecl DecomposedDecl => _decomposedDecl.Value;

    public VarDecl HoldingVar => _holdingVar.Value;
}
