// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class RecordDecl : TagDecl
    {
        private readonly Lazy<IReadOnlyList<Decl>> _anonymousDecls;
        private readonly Lazy<IReadOnlyList<FieldDecl>> _fields;

        internal RecordDecl(CXCursor handle) : this(handle, handle.Kind, CX_DeclKind.CX_DeclKind_Record)
        {
        }

        private protected RecordDecl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind, expectedDeclKind)
        {
            if ((CX_DeclKind.CX_DeclKind_LastRecord < handle.DeclKind) || (handle.DeclKind < CX_DeclKind.CX_DeclKind_FirstRecord))
            {
                throw new ArgumentException(nameof(handle));
            }

            if ((handle.Kind != CXCursorKind.CXCursor_StructDecl) && (handle.Kind != CXCursorKind.CXCursor_UnionDecl) && (handle.Kind != CXCursorKind.CXCursor_ClassDecl) && (handle.Kind != CXCursorKind.CXCursor_ClassTemplatePartialSpecialization))
            {
                throw new ArgumentException(nameof(handle));
            }

            _fields = new Lazy<IReadOnlyList<FieldDecl>>(() => Decls.OfType<FieldDecl>().ToList());
            _anonymousDecls = new Lazy<IReadOnlyList<Decl>>(() => Decls.Where(decl => ((decl is FieldDecl field) && field.IsAnonymousField) || ((decl is RecordDecl record) && record.IsAnonymousStructOrUnion)).ToList());
        }

        public bool IsAnonymousStructOrUnion => Handle.IsAnonymousStructOrUnion;

        public IReadOnlyList<Decl> AnonymousDecls => _anonymousDecls.Value;

        public new RecordDecl Definition => (RecordDecl)base.Definition;

        public IReadOnlyList<FieldDecl> Fields => _fields.Value;

        public new RecordDecl MostRecentDecl => (RecordDecl)base.MostRecentDecl;

        public new RecordDecl PreviousDecl => (RecordDecl)base.PreviousDecl;
    }
}
