// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class IndirectFieldDecl : ValueDecl, IMergeable<IndirectFieldDecl>
    {
        private readonly Lazy<FieldDecl> _anonField;
        private readonly Lazy<VarDecl> _varDecl;

        internal IndirectFieldDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnexposedDecl, CX_DeclKind.CX_DeclKind_IndirectField)
        {
            _anonField = new Lazy<FieldDecl>(() => TranslationUnit.GetOrCreate<FieldDecl>(Handle.GetSubDecl(0)));
            _varDecl = new Lazy<VarDecl>(() => TranslationUnit.GetOrCreate<VarDecl>(Handle.GetSubDecl(1)));
        }

        public FieldDecl AnonField => _anonField.Value;

        public VarDecl VarDecl => _varDecl.Value;
    }
}
