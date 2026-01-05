// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public sealed class ParmVarDecl : VarDecl
{
    private readonly ValueLazy<Expr> _defaultArg;
    private readonly ValueLazy<Type> _originalType;
    private readonly ValueLazy<Expr> _uninstantiatedDefaultArg;

    internal ParmVarDecl(CXCursor handle) : base(handle, CXCursor_ParmDecl, CX_DeclKind_ParmVar)
    {
        _defaultArg = new ValueLazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.DefaultArg));
        _originalType = new ValueLazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.OriginalType));
        _uninstantiatedDefaultArg = new ValueLazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.UninstantiatedDefaultArg));
    }

    public Expr DefaultArg => _defaultArg.Value;

    public int FunctionScopeDepth => Handle.FunctionScopeDepth;

    public int FunctionScopeIndex => Handle.FunctionScopeIndex;

    public bool HasDefaultArg => Handle.HasDefaultArg;

    public bool HasUnparsedDefaultArg => Handle.HasUnparsedDefaultArg;

    public bool HasUninstantiatedDefaultArg => Handle.HasUninstantiatedDefaultArg;

    public bool HasInheritedDefaultArg => Handle.HasInheritedDefaultArg;

    public Type OriginalType => _originalType.Value;

    public Expr UninstantiatedDefaultArg => _uninstantiatedDefaultArg.Value;
}
