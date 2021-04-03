// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class RecordDecl : TagDecl
    {
        private readonly Lazy<IReadOnlyList<FieldDecl>> _anonymousFields;
        private readonly Lazy<IReadOnlyList<RecordDecl>> _anonymousRecords;
        private readonly Lazy<IReadOnlyList<FieldDecl>> _fields;
        private readonly Lazy<IReadOnlyList<IndirectFieldDecl>> _indirectFields;
        private readonly Lazy<RecordDecl> _injectedClassName;
        

        internal RecordDecl(CXCursor handle) : this(handle, handle.Kind, CX_DeclKind.CX_DeclKind_Record)
        {
        }

        private protected RecordDecl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind, expectedDeclKind)
        {
            if (handle.DeclKind is > CX_DeclKind.CX_DeclKind_LastRecord or < CX_DeclKind.CX_DeclKind_FirstRecord)
            {
                throw new ArgumentOutOfRangeException(nameof(handle));
            }

            if (handle.Kind is not CXCursorKind.CXCursor_StructDecl and not CXCursorKind.CXCursor_UnionDecl and not CXCursorKind.CXCursor_ClassDecl and not CXCursorKind.CXCursor_ClassTemplatePartialSpecialization)
            {
                throw new ArgumentOutOfRangeException(nameof(handle));
            }

            _fields = new Lazy<IReadOnlyList<FieldDecl>>(() => {
                var numFields = Handle.NumFields;
                var fields = new List<FieldDecl>(numFields);

                for (var i = 0; i < numFields; i++)
                {
                    var field = TranslationUnit.GetOrCreate<FieldDecl>(Handle.GetField(unchecked((uint)i)));
                    fields.Add(field);
                }

                return fields;
            });

            _anonymousFields = new Lazy<IReadOnlyList<FieldDecl>>(() => Decls.OfType<FieldDecl>().Where(decl => decl.IsAnonymousField).ToList());
            _anonymousRecords = new Lazy<IReadOnlyList<RecordDecl>>(() => Decls.OfType<RecordDecl>().Where(decl => decl.IsAnonymousStructOrUnion && !decl.IsInjectedClassName).ToList());
            _indirectFields = new Lazy<IReadOnlyList<IndirectFieldDecl>>(() => Decls.OfType<IndirectFieldDecl>().ToList());
            _injectedClassName = new Lazy<RecordDecl>(() => Decls.OfType<RecordDecl>().Where(decl => decl.IsInjectedClassName).SingleOrDefault());
        }

        public bool IsAnonymousStructOrUnion => Handle.IsAnonymousStructOrUnion;

        public IReadOnlyList<FieldDecl> AnonymousFields => _anonymousFields.Value;

        public IReadOnlyList<RecordDecl> AnonymousRecords => _anonymousRecords.Value;

        public new RecordDecl Definition => (RecordDecl)base.Definition;

        public IReadOnlyList<FieldDecl> Fields => _fields.Value;

        public IReadOnlyList<IndirectFieldDecl> IndirectFields => _indirectFields.Value;

        public RecordDecl InjectedClassName => _injectedClassName.Value;

        public bool IsInjectedClassName => Handle.IsInjectedClassName;

        public new RecordDecl MostRecentDecl => (RecordDecl)base.MostRecentDecl;

        public new RecordDecl PreviousDecl => (RecordDecl)base.PreviousDecl;
    }
}
