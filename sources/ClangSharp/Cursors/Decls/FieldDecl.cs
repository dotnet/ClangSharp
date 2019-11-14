// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class FieldDecl : DeclaratorDecl, IMergeable<FieldDecl>
    {
        internal FieldDecl(CXCursor handle) : this(handle, CXCursorKind.CXCursor_FieldDecl, CX_DeclKind.CX_DeclKind_Field)
        {
        }

        private protected FieldDecl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind, expectedDeclKind)
        {
            if ((CX_DeclKind.CX_DeclKind_LastField < handle.DeclKind) || (handle.DeclKind < CX_DeclKind.CX_DeclKind_FirstField))
            {
                throw new ArgumentException(nameof(handle));
            }
        }

        public int BitWidthValue => Handle.FieldDeclBitWidth;

        public bool IsBitField => Handle.IsBitField;

        public bool IsMutable => Handle.CXXField_IsMutable;

        public RecordDecl Parent => (RecordDecl)CursorParent;
    }
}
