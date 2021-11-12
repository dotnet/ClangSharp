// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

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
