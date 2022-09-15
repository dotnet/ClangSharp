// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;

namespace ClangSharp;

public class FieldDecl : DeclaratorDecl, IMergeable<FieldDecl>
{
    private readonly Lazy<Expr> _bitWidth;
    private readonly Lazy<Expr> _inClassInitializer;

    internal FieldDecl(CXCursor handle) : this(handle, CXCursorKind.CXCursor_FieldDecl, CX_DeclKind.CX_DeclKind_Field)
    {
    }

    private protected FieldDecl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind, expectedDeclKind)
    {
        if (handle.DeclKind is > CX_DeclKind.CX_DeclKind_LastField or < CX_DeclKind.CX_DeclKind_FirstField)
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
        }

        _bitWidth = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.BitWidth));
        _inClassInitializer = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.InClassInitializer));
    }

    public Expr BitWidth => _bitWidth.Value;

    public int BitWidthValue => Handle.FieldDeclBitWidth;

    public new FieldDecl CanonicalDecl => (FieldDecl)base.CanonicalDecl;

    public int FieldIndex => Handle.FieldIndex;

    public Expr InClassInitializer => _inClassInitializer.Value;

    public bool IsAnonymousField => string.IsNullOrWhiteSpace(Name);

    public bool IsAnonymousStructOrUnion => Handle.IsAnonymousStructOrUnion;

    public bool IsBitField => Handle.IsBitField;

    public bool IsMutable => Handle.CXXField_IsMutable;

    public bool IsUnnamedBitfield => Handle.IsUnnamedBitfield;

    public new RecordDecl Parent => (RecordDecl)DeclContext ?? ((SemanticParentCursor is ClassTemplateDecl classTemplateDecl) ? (RecordDecl)classTemplateDecl.TemplatedDecl : null);
}
