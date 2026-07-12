// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public sealed class ParmVarDecl : VarDecl
{
    private ValueLazy<ParmVarDecl, Expr> _defaultArg;
    private ValueLazy<ParmVarDecl, Type> _originalType;
    private ValueLazy<ParmVarDecl, Expr> _uninstantiatedDefaultArg;

    internal unsafe ParmVarDecl(CXCursor handle) : base(handle, CXCursor_ParmDecl, CX_DeclKind_ParmVar)
    {
        _defaultArg = new ValueLazy<ParmVarDecl, Expr>(&DefaultArgFactory);
        _originalType = new ValueLazy<ParmVarDecl, Type>(&OriginalTypeFactory);
        _uninstantiatedDefaultArg = new ValueLazy<ParmVarDecl, Expr>(&UninstantiatedDefaultArgFactory);
    }

    public Expr DefaultArg => _defaultArg.GetValue(this);

    public int FunctionScopeDepth => Handle.FunctionScopeDepth;

    public int FunctionScopeIndex => Handle.FunctionScopeIndex;

    public bool HasDefaultArg => Handle.HasDefaultArg;

    public bool HasUnparsedDefaultArg => Handle.HasUnparsedDefaultArg;

    public bool HasUninstantiatedDefaultArg => Handle.HasUninstantiatedDefaultArg;

    public bool HasInheritedDefaultArg => Handle.HasInheritedDefaultArg;

    public Type OriginalType => _originalType.GetValue(this);

    public Expr UninstantiatedDefaultArg => _uninstantiatedDefaultArg.GetValue(this);

    private static unsafe Expr UninstantiatedDefaultArgFactory(ParmVarDecl self) => self.TranslationUnit.GetOrCreate<Expr>(self.Handle.UninstantiatedDefaultArg);

    private static unsafe Type OriginalTypeFactory(ParmVarDecl self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.OriginalType);

    private static unsafe Expr DefaultArgFactory(ParmVarDecl self) => self.TranslationUnit.GetOrCreate<Expr>(self.Handle.DefaultArg);
}
