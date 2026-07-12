// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_DeclKind;
using static ClangSharp.Interop.CXCursorKind;

namespace ClangSharp;

public class RecordDecl : TagDecl
{
    private ValueLazy<RecordDecl, List<FieldDecl>> _anonymousFields;
    private ValueLazy<RecordDecl, List<RecordDecl>> _anonymousRecords;
    private readonly LazyList<FieldDecl> _fields;
    private ValueLazy<RecordDecl, List<IndirectFieldDecl>> _indirectFields;
    private ValueLazy<RecordDecl, RecordDecl?> _injectedClassName;
    

    internal RecordDecl(CXCursor handle) : this(handle, handle.Kind, CX_DeclKind_Record)
    {
    }

    private protected unsafe RecordDecl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind, expectedDeclKind)
    {
        if (handle.DeclKind is > CX_DeclKind_LastRecord or < CX_DeclKind_FirstRecord)
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
        }

        if (handle.Kind is not CXCursor_StructDecl and not CXCursor_UnionDecl and not CXCursor_ClassDecl and not CXCursor_ClassTemplatePartialSpecialization)
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
        }

        _fields = LazyList.Create<FieldDecl>(Handle.NumFields, (i) => TranslationUnit.GetOrCreate<FieldDecl>(Handle.GetField(unchecked((uint)i))));
        _anonymousFields = new ValueLazy<RecordDecl, List<FieldDecl>>(&AnonymousFieldsFactory);
        _anonymousRecords = new ValueLazy<RecordDecl, List<RecordDecl>>(&AnonymousRecordsFactory);
        _indirectFields = new ValueLazy<RecordDecl, List<IndirectFieldDecl>>(&IndirectFieldsFactory);
        _injectedClassName = new ValueLazy<RecordDecl, RecordDecl?>(&InjectedClassNameFactory);
    }

    public bool IsAnonymousStructOrUnion => Handle.IsAnonymousStructOrUnion;

    public IReadOnlyList<FieldDecl> AnonymousFields => _anonymousFields.GetValue(this);

    public IReadOnlyList<RecordDecl> AnonymousRecords => _anonymousRecords.GetValue(this);

    public new RecordDecl? Definition => (RecordDecl?)base.Definition;

    public IReadOnlyList<FieldDecl> Fields => _fields;

    public IReadOnlyList<IndirectFieldDecl> IndirectFields => _indirectFields.GetValue(this);

    public RecordDecl? InjectedClassName => _injectedClassName.GetValue(this);

    public bool IsInjectedClassName => Handle.IsInjectedClassName;

    public new RecordDecl MostRecentDecl => (RecordDecl)base.MostRecentDecl;

    public new RecordDecl PreviousDecl => (RecordDecl)base.PreviousDecl;

    private static unsafe RecordDecl? InjectedClassNameFactory(RecordDecl self) => self.Decls.OfType<RecordDecl>().Where(decl => decl.IsInjectedClassName).SingleOrDefault();

    private static unsafe List<IndirectFieldDecl> IndirectFieldsFactory(RecordDecl self) => [.. self.Decls.OfType<IndirectFieldDecl>()];

    private static unsafe List<RecordDecl> AnonymousRecordsFactory(RecordDecl self) => [.. self.Decls.OfType<RecordDecl>().Where(decl => decl.IsAnonymousStructOrUnion && !decl.IsInjectedClassName)];

    private static unsafe List<FieldDecl> AnonymousFieldsFactory(RecordDecl self) => [.. self.Decls.OfType<FieldDecl>().Where(decl => decl.IsAnonymousField)];
}
