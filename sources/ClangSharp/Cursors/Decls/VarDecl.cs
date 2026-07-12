// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public class VarDecl : DeclaratorDecl, IRedeclarable<VarDecl>
{
    private ValueLazy<VarDecl, VarDecl> _definition;
    private ValueLazy<VarDecl, Expr> _init;
    private ValueLazy<VarDecl, VarDecl> _instantiatedFromStaticDataMember;

    internal VarDecl(CXCursor handle) : this(handle, CXCursor_VarDecl, CX_DeclKind_Var)
    {
    }

    private protected unsafe VarDecl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind, expectedDeclKind)
    {
        if (handle.DeclKind is > CX_DeclKind_LastVar or < CX_DeclKind_FirstVar)
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
        }

        _definition = new ValueLazy<VarDecl, VarDecl>(&DefinitionFactory);
        _init = new ValueLazy<VarDecl, Expr>(&InitFactory);
        _instantiatedFromStaticDataMember = new ValueLazy<VarDecl, VarDecl>(&InstantiatedFromStaticDataMemberFactory);
    }

    public new VarDecl CanonicalDecl => (VarDecl)base.CanonicalDecl;

    public VarDecl Definition => _definition.GetValue(this);

    public bool HasExternalStorage => Handle.HasVarDeclExternalStorage;

    public bool HasGlobalStorage => Handle.HasVarDeclGlobalStorage;

    public bool HasInit => Handle.HasInit;

    public bool HasLocalStorage => Handle.HasLocalStorage;

    public Expr Init => _init.GetValue(this);

    public VarDecl InstantiatedFromStaticDataMember => _instantiatedFromStaticDataMember.GetValue(this);

    public bool IsExternC => Handle.IsExternC;

    public bool IsLocalVarDecl => Handle.IsLocalVarDecl;

    public bool IsLocalVarDeclOrParm => Handle.IsLocalVarDeclOrParm;

    public bool IsStaticDataMember => Handle.IsStaticDataMember;

    public CX_StorageClass StorageClass => Handle.StorageClass;

    public CXTLSKind TlsKind => Handle.TlsKind;

    private static unsafe VarDecl InstantiatedFromStaticDataMemberFactory(VarDecl self) => self.TranslationUnit.GetOrCreate<VarDecl>(self.Handle.InstantiatedFromMember);

    private static unsafe Expr InitFactory(VarDecl self) => self.TranslationUnit.GetOrCreate<Expr>(self.Handle.InitExpr);

    private static unsafe VarDecl DefinitionFactory(VarDecl self) => self.TranslationUnit.GetOrCreate<VarDecl>(self.Handle.Definition);
}
