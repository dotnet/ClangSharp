// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class VarDecl : DeclaratorDecl, IRedeclarable<VarDecl>
    {
        private readonly Lazy<VarDecl> _instantiatedFromStaticDataMember;

        internal VarDecl(CXCursor handle) : this(handle, CXCursorKind.CXCursor_VarDecl)
        {
        }

        private protected VarDecl(CXCursor handle, CXCursorKind expectedKind) : base(handle, expectedKind)
        {
            _instantiatedFromStaticDataMember = new Lazy<VarDecl>(() => TranslationUnit.GetOrCreate<VarDecl>(Handle.SpecializedCursorTemplate));
        }

        public VarDecl InstantiatedFromStaticDataMember => _instantiatedFromStaticDataMember.Value;

        public CX_StorageClass StorageClass => Handle.StorageClass;

        public CXTLSKind TlsKind => Handle.TlsKind;
    }
}
