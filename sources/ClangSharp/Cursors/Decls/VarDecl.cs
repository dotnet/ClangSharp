// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public class VarDecl : DeclaratorDecl, IRedeclarable<VarDecl>
{
    private readonly Lazy<VarDecl> _definition;
    private readonly Lazy<Expr> _init;
    private readonly Lazy<VarDecl> _instantiatedFromStaticDataMember;

    internal VarDecl(CXCursor handle) : this(handle, CXCursor_VarDecl, CX_DeclKind_Var)
    {
    }

    private protected VarDecl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind, expectedDeclKind)
    {
        if (handle.DeclKind is > CX_DeclKind_LastVar or < CX_DeclKind_FirstVar)
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
        }

        _definition = new Lazy<VarDecl>(() => TranslationUnit.GetOrCreate<VarDecl>(Handle.Definition));
        _init = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(handle.InitExpr));
        _instantiatedFromStaticDataMember = new Lazy<VarDecl>(() => TranslationUnit.GetOrCreate<VarDecl>(Handle.InstantiatedFromMember));
    }

    public new VarDecl CanonicalDecl => (VarDecl)base.CanonicalDecl;

    public VarDecl Definition => _definition.Value;

    public bool HasExternalStorage => Handle.HasVarDeclExternalStorage;

    public bool HasGlobalStorage => Handle.HasVarDeclGlobalStorage;

    public bool HasInit => Handle.HasInit;

    public bool HasLocalStorage => Handle.HasLocalStorage;

    public Expr Init => _init.Value;

    public VarDecl InstantiatedFromStaticDataMember => _instantiatedFromStaticDataMember.Value;

    public bool IsExternC => Handle.IsExternC;

    public bool IsLocalVarDecl => Handle.IsLocalVarDecl;

    public bool IsLocalVarDeclOrParm => Handle.IsLocalVarDeclOrParm;

    public bool IsStaticDataMember => Handle.IsStaticDataMember;

    public CX_StorageClass StorageClass => Handle.StorageClass;

    public CXTLSKind TlsKind => Handle.TlsKind;
}
