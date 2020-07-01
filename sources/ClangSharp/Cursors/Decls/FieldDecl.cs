// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class FieldDecl : DeclaratorDecl, IMergeable<FieldDecl>
    {
        private readonly Lazy<Expr> _bitWidth;
        private readonly Lazy<Expr> _inClassInitializer;

        internal FieldDecl(CXCursor handle) : this(handle, CXCursorKind.CXCursor_FieldDecl, CX_DeclKind.CX_DeclKind_Field)
        {
        }

        private protected FieldDecl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind, expectedDeclKind)
        {
            if ((CX_DeclKind.CX_DeclKind_LastField < handle.DeclKind) || (handle.DeclKind < CX_DeclKind.CX_DeclKind_FirstField))
            {
                throw new ArgumentException(nameof(handle));
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

        public RecordDecl Parent => (RecordDecl)DeclContext;
    }
}
