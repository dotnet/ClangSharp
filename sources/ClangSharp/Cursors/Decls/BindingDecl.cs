// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public sealed class BindingDecl : ValueDecl
{
    private ValueLazy<BindingDecl, Expr> _binding;
    private ValueLazy<BindingDecl, ValueDecl> _decomposedDecl;
    private ValueLazy<BindingDecl, VarDecl> _holdingVar;

    internal unsafe BindingDecl(CXCursor handle) : base(handle, CXCursor_UnexposedDecl, CX_DeclKind_Binding)
    {
        _binding = new ValueLazy<BindingDecl, Expr>(&BindingFactory);
        _decomposedDecl = new ValueLazy<BindingDecl, ValueDecl>(&DecomposedDeclFactory);
        _holdingVar = new ValueLazy<BindingDecl, VarDecl>(&HoldingVarFactory);
    }

    public Expr Binding => _binding.GetValue(this);

    public ValueDecl DecomposedDecl => _decomposedDecl.GetValue(this);

    public VarDecl HoldingVar => _holdingVar.GetValue(this);

    private static unsafe VarDecl HoldingVarFactory(BindingDecl self) => self.TranslationUnit.GetOrCreate<VarDecl>(self.Handle.GetSubDecl(0));

    private static unsafe ValueDecl DecomposedDeclFactory(BindingDecl self) => self.TranslationUnit.GetOrCreate<ValueDecl>(self.Handle.DecomposedDecl);

    private static unsafe Expr BindingFactory(BindingDecl self) => self.TranslationUnit.GetOrCreate<Expr>(self.Handle.BindingExpr);
}
