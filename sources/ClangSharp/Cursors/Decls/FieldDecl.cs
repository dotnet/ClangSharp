// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using ClangSharp.Interop;

namespace ClangSharp
{
    public class FieldDecl : DeclaratorDecl, IMergeable<FieldDecl>
    {
        internal FieldDecl(CXCursor handle) : this(handle, CXCursorKind.CXCursor_FieldDecl)
        {
        }

        private protected FieldDecl(CXCursor handle, CXCursorKind expectedKind) : base(handle, expectedKind)
        {
        }

        public int BitWidthValue => Handle.FieldDeclBitWidth;

        public bool IsBitField => Handle.IsBitField;

        public bool IsMutable => Handle.CXXField_IsMutable;

        public RecordDecl Parent => (RecordDecl)CursorParent;
    }
}
