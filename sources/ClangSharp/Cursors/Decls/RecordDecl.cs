// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public class RecordDecl : TagDecl
{
    private readonly Lazy<IReadOnlyList<FieldDecl>> _anonymousFields;
    private readonly Lazy<IReadOnlyList<RecordDecl>> _anonymousRecords;
    private readonly Lazy<IReadOnlyList<FieldDecl>> _fields;
    private readonly Lazy<IReadOnlyList<IndirectFieldDecl>> _indirectFields;
    private readonly Lazy<RecordDecl?> _injectedClassName;
    

    internal RecordDecl(CXCursor handle) : this(handle, handle.Kind, CX_DeclKind_Record)
    {
    }

    private protected RecordDecl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind, expectedDeclKind)
    {
        if (handle.DeclKind is > CX_DeclKind_LastRecord or < CX_DeclKind_FirstRecord)
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
        }

        if (handle.Kind is not CXCursor_StructDecl and not CXCursor_UnionDecl and not CXCursor_ClassDecl and not CXCursor_ClassTemplatePartialSpecialization)
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
        _injectedClassName = new Lazy<RecordDecl?>(() => Decls.OfType<RecordDecl>().Where(decl => decl.IsInjectedClassName).SingleOrDefault());
    }

    public bool IsAnonymousStructOrUnion => Handle.IsAnonymousStructOrUnion;

    public IReadOnlyList<FieldDecl> AnonymousFields => _anonymousFields.Value;

    public IReadOnlyList<RecordDecl> AnonymousRecords => _anonymousRecords.Value;

    public new RecordDecl? Definition => (RecordDecl?)base.Definition;

    public IReadOnlyList<FieldDecl> Fields => _fields.Value;

    public IReadOnlyList<IndirectFieldDecl> IndirectFields => _indirectFields.Value;

    public RecordDecl? InjectedClassName => _injectedClassName.Value;

    public bool IsInjectedClassName => Handle.IsInjectedClassName;

    public new RecordDecl MostRecentDecl => (RecordDecl)base.MostRecentDecl;

    public new RecordDecl PreviousDecl => (RecordDecl)base.PreviousDecl;
}
