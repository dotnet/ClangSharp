// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class VarDecl : DeclaratorDecl, IRedeclarable<VarDecl>
    {
        private readonly Lazy<VarDecl> _instantiatedFromStaticDataMember;

        internal VarDecl(CXCursor handle) : this(handle, CXCursorKind.CXCursor_VarDecl, CX_DeclKind.CX_DeclKind_Var)
        {
        }

        private protected VarDecl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind, expectedDeclKind)
        {
            if ((CX_DeclKind.CX_DeclKind_LastVar < handle.DeclKind) || (handle.DeclKind < CX_DeclKind.CX_DeclKind_FirstVar))
            {
                throw new ArgumentException(nameof(handle));
            }

            _instantiatedFromStaticDataMember = new Lazy<VarDecl>(() => TranslationUnit.GetOrCreate<VarDecl>(Handle.SpecializedCursorTemplate));
        }

        public VarDecl InstantiatedFromStaticDataMember => _instantiatedFromStaticDataMember.Value;

        public CX_StorageClass StorageClass => Handle.StorageClass;

        public CXTLSKind TlsKind => Handle.TlsKind;
    }
}
