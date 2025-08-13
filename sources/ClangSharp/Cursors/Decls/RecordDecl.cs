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
    private readonly Lazy<List<FieldDecl>> _anonymousFields;
    private readonly Lazy<List<RecordDecl>> _anonymousRecords;
    private readonly LazyList<FieldDecl> _fields;
    private readonly Lazy<List<IndirectFieldDecl>> _indirectFields;
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

        _fields = LazyList.Create<FieldDecl>(Handle.NumFields, (i) => TranslationUnit.GetOrCreate<FieldDecl>(Handle.GetField(unchecked((uint)i))));
        _anonymousFields = new Lazy<List<FieldDecl>>(() => [.. Decls.OfType<FieldDecl>().Where(decl => decl.IsAnonymousField)]);
        _anonymousRecords = new Lazy<List<RecordDecl>>(() => [.. Decls.OfType<RecordDecl>().Where(decl => decl.IsAnonymous && !decl.IsInjectedClassName)]);
        _indirectFields = new Lazy<List<IndirectFieldDecl>>(() => [.. Decls.OfType<IndirectFieldDecl>()]);
        _injectedClassName = new Lazy<RecordDecl?>(() => Decls.OfType<RecordDecl>().Where(decl => decl.IsInjectedClassName).SingleOrDefault());
    }

    public bool IsAnonymous => Handle.IsAnonymous;

    public IReadOnlyList<FieldDecl> AnonymousFields => _anonymousFields.Value;

    public IReadOnlyList<RecordDecl> AnonymousRecords => _anonymousRecords.Value;

    public new RecordDecl? Definition => (RecordDecl?)base.Definition;

    public IReadOnlyList<FieldDecl> Fields => _fields;

    public IReadOnlyList<IndirectFieldDecl> IndirectFields => _indirectFields.Value;

    public RecordDecl? InjectedClassName => _injectedClassName.Value;

    public bool IsInjectedClassName => Handle.IsInjectedClassName;

    public new RecordDecl MostRecentDecl => (RecordDecl)base.MostRecentDecl;

    public new RecordDecl PreviousDecl => (RecordDecl)base.PreviousDecl;
}
