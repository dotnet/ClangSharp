// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_DeclKind;
using static ClangSharp.Interop.CXCursorKind;

namespace ClangSharp;

public unsafe class TagDecl : TypeDecl, IDeclContext, IRedeclarable<TagDecl>
{
    private ValueLazy<TagDecl, TagDecl?> _definition;
    private readonly LazyList<LazyList<NamedDecl>> _templateParameterLists;
    private ValueLazy<TagDecl, TypedefNameDecl?> _typedefNameForAnonDecl;

    private protected TagDecl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind, expectedDeclKind)
    {
        if (handle.DeclKind is > CX_DeclKind_LastTag or < CX_DeclKind_FirstTag)
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
        }

        _definition = new ValueLazy<TagDecl, TagDecl?>(&DefinitionFactory);
        _templateParameterLists = CreateTemplateParameterLists(this);
        _typedefNameForAnonDecl = new ValueLazy<TagDecl, TypedefNameDecl?>(&TypedefNameForAnonDeclFactory);
    }

    public new TagDecl CanonicalDecl => (TagDecl)base.CanonicalDecl;

    public TagDecl? Definition => _definition.GetValue(this);

    public bool IsClass => CursorKind == CXCursor_ClassDecl;

    public bool IsCompleteDefinition => Handle.IsCompleteDefinition;

    public bool IsEnum => CursorKind == CXCursor_EnumDecl;

    public bool IsStruct => CursorKind == CXCursor_StructDecl;

    public bool IsThisDeclarationADefinition => Handle.IsThisDeclarationADefinition;

    public bool IsUnion => CursorKind == CXCursor_UnionDecl;

    public uint NumTemplateParameterLists => unchecked((uint)Handle.NumTemplateParameterLists);

    public IReadOnlyList<IReadOnlyList<NamedDecl>> TemplateParameterLists => _templateParameterLists;

    public TypedefNameDecl? TypedefNameForAnonDecl => _typedefNameForAnonDecl.GetValue(this);

    private static unsafe TypedefNameDecl? TypedefNameForAnonDeclFactory(TagDecl self) => !self.Handle.TypedefNameForAnonDecl.IsNull ? self.TranslationUnit.GetOrCreate<TypedefNameDecl>(self.Handle.TypedefNameForAnonDecl) : null;

    private static unsafe TagDecl? DefinitionFactory(TagDecl self) => !self.Handle.Definition.IsNull ? self.TranslationUnit.GetOrCreate<TagDecl>(self.Handle.Definition) : null;
}
