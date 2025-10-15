// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_DeclKind;
using System.Diagnostics;

namespace ClangSharp;

public class FieldDecl : DeclaratorDecl, IMergeable<FieldDecl>
{
    private readonly ValueLazy<Expr> _bitWidth;
    private readonly ValueLazy<Expr> _inClassInitializer;
    private readonly ValueLazy<bool> _isAnonymousField;

    internal FieldDecl(CXCursor handle) : this(handle, CXCursor_FieldDecl, CX_DeclKind_Field)
    {
    }

    private protected FieldDecl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind, expectedDeclKind)
    {
        if (handle.DeclKind is > CX_DeclKind_LastField or < CX_DeclKind_FirstField)
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
        }

        _bitWidth = new ValueLazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.BitWidth));
        _inClassInitializer = new ValueLazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.InClassInitializer));
        _isAnonymousField = new ValueLazy<bool>(() => {
            var name = Name.AsSpan();

            if (name.IsWhiteSpace())
            {
                return true;
            }

            var anonymousNameStartIndex = name.IndexOf("::(", StringComparison.Ordinal);

            if (anonymousNameStartIndex != -1)
            {
                anonymousNameStartIndex += 2;
                name = name[anonymousNameStartIndex..];
            }

#if NET8_0
            if (name.StartsWith("("))
#else
            if (name.StartsWith('('))
#endif
            {
                Debug.Assert(name.StartsWith("(anonymous enum at ", StringComparison.Ordinal) ||
                                 name.StartsWith("(anonymous struct at ", StringComparison.Ordinal) ||
                                 name.StartsWith("(anonymous union at ", StringComparison.Ordinal) ||
                                 name.StartsWith("(unnamed enum at ", StringComparison.Ordinal) ||
                                 name.StartsWith("(unnamed struct at ", StringComparison.Ordinal) ||
                                 name.StartsWith("(unnamed union at ", StringComparison.Ordinal) ||
                                 name.StartsWith("(unnamed at ", StringComparison.Ordinal));

#if NET8_0
                Debug.Assert(name.EndsWith(")"));
#else
                Debug.Assert(name.EndsWith(')'));
#endif

                return true;
            }

            return false;
        });
    }

    public Expr BitWidth => _bitWidth.Value;

    public int BitWidthValue => Handle.FieldDeclBitWidth;

    public new FieldDecl CanonicalDecl => (FieldDecl)base.CanonicalDecl;

    public int FieldIndex => Handle.FieldIndex;

    public Expr InClassInitializer => _inClassInitializer.Value;

    public bool IsAnonymousField => _isAnonymousField.Value;

    public bool IsAnonymousStructOrUnion => Handle.IsAnonymousStructOrUnion;

    public bool IsBitField => Handle.IsBitField;

    public bool IsMutable => Handle.CXXField_IsMutable;

    public bool IsUnnamedBitfield => Handle.IsUnnamedBitfield;

    public new RecordDecl? Parent => (DeclContext as RecordDecl) ?? ((SemanticParentCursor is ClassTemplateDecl classTemplateDecl) ? (RecordDecl)classTemplateDecl.TemplatedDecl : null);
}
