// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public sealed class IndirectFieldDecl : ValueDecl, IMergeable<IndirectFieldDecl>
{
    private ValueLazy<IndirectFieldDecl, FieldDecl> _anonField;
    private ValueLazy<IndirectFieldDecl, VarDecl> _varDecl;

    internal unsafe IndirectFieldDecl(CXCursor handle) : base(handle, CXCursor_UnexposedDecl, CX_DeclKind_IndirectField)
    {
        _anonField = new ValueLazy<IndirectFieldDecl, FieldDecl>(&AnonFieldFactory);
        _varDecl = new ValueLazy<IndirectFieldDecl, VarDecl>(&VarDeclFactory);
    }

    public FieldDecl AnonField => _anonField.GetValue(this);

    public VarDecl VarDecl => _varDecl.GetValue(this);

    private static unsafe VarDecl VarDeclFactory(IndirectFieldDecl self) => self.TranslationUnit.GetOrCreate<VarDecl>(self.Handle.GetSubDecl(1));

    private static unsafe FieldDecl AnonFieldFactory(IndirectFieldDecl self) => self.TranslationUnit.GetOrCreate<FieldDecl>(self.Handle.GetSubDecl(0));
}
