// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_DeclKind;
using static ClangSharp.Interop.CXCursorKind;

namespace ClangSharp;

public unsafe class TagDecl : TypeDecl, IDeclContext, IRedeclarable<TagDecl>
{
    private readonly ValueLazy<TagDecl?> _definition;
    private readonly LazyList<LazyList<NamedDecl>> _templateParameterLists;
    private readonly ValueLazy<TypedefNameDecl?> _typedefNameForAnonDecl;

    private protected TagDecl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind, expectedDeclKind)
    {
        if (handle.DeclKind is > CX_DeclKind_LastTag or < CX_DeclKind_FirstTag)
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
        }

        _definition = new ValueLazy<TagDecl?>(() => !Handle.Definition.IsNull ? TranslationUnit.GetOrCreate<TagDecl>(Handle.Definition) : null);
        _templateParameterLists = LazyList.Create<LazyList<NamedDecl>>(Handle.NumTemplateParameterLists, (listIndex) => {
            var numTemplateParameters = Handle.GetNumTemplateParameters(unchecked((uint)listIndex));
            return LazyList.Create<NamedDecl>(numTemplateParameters, (parameterIndex) => TranslationUnit.GetOrCreate<NamedDecl>(Handle.GetTemplateParameter(unchecked((uint)listIndex), unchecked((uint)parameterIndex))));
        });
        _typedefNameForAnonDecl = new ValueLazy<TypedefNameDecl?>(() => !Handle.TypedefNameForAnonDecl.IsNull ? TranslationUnit.GetOrCreate<TypedefNameDecl>(Handle.TypedefNameForAnonDecl) : null);
    }

    public new TagDecl CanonicalDecl => (TagDecl)base.CanonicalDecl;

    public TagDecl? Definition => _definition.Value;

    public bool IsClass => CursorKind == CXCursor_ClassDecl;

    public bool IsCompleteDefinition => Handle.IsCompleteDefinition;

    public bool IsEnum => CursorKind == CXCursor_EnumDecl;

    public bool IsStruct => CursorKind == CXCursor_StructDecl;

    public bool IsThisDeclarationADefinition => Handle.IsThisDeclarationADefinition;

    public bool IsUnion => CursorKind == CXCursor_UnionDecl;

    public uint NumTemplateParameterLists => unchecked((uint)Handle.NumTemplateParameterLists);

    public IReadOnlyList<IReadOnlyList<NamedDecl>> TemplateParameterLists => _templateParameterLists;

    public TypedefNameDecl? TypedefNameForAnonDecl => _typedefNameForAnonDecl.Value;
}
