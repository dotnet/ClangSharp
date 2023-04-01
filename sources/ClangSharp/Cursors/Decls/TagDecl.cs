// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public unsafe class TagDecl : TypeDecl, IDeclContext, IRedeclarable<TagDecl>
{
    private readonly Lazy<TagDecl?> _definition;
    private readonly Lazy<IReadOnlyList<IReadOnlyList<NamedDecl>>> _templateParameterLists;
    private readonly Lazy<TypedefNameDecl?> _typedefNameForAnonDecl;

    private protected TagDecl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind, expectedDeclKind)
    {
        if (handle.DeclKind is > CX_DeclKind_LastTag or < CX_DeclKind_FirstTag)
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
        }

        _definition = new Lazy<TagDecl?>(() => !Handle.Definition.IsNull ? TranslationUnit.GetOrCreate<TagDecl>(Handle.Definition) : null);

        _templateParameterLists = new Lazy<IReadOnlyList<IReadOnlyList<NamedDecl>>>(() => {
            var numTemplateParameterLists = Handle.NumTemplateParameterLists;
            var templateParameterLists = new List<IReadOnlyList<NamedDecl>>(numTemplateParameterLists);

            for (var listIndex = 0; listIndex < numTemplateParameterLists; listIndex++)
            {
                var numTemplateParameters = Handle.GetNumTemplateParameters(unchecked((uint)listIndex));
                var templateParameterList = new List<NamedDecl>(numTemplateParameters);

                for (var parameterIndex = 0; parameterIndex < numTemplateParameters; parameterIndex++)
                {
                    var templateParameter = TranslationUnit.GetOrCreate<NamedDecl>(Handle.GetTemplateParameter(unchecked((uint)listIndex), unchecked((uint)parameterIndex)));
                    templateParameterList.Add(templateParameter);
                }

                templateParameterLists.Add(templateParameterList);
            }

            return templateParameterLists;
        });

        _typedefNameForAnonDecl = new Lazy<TypedefNameDecl?>(() => !Handle.TypedefNameForAnonDecl.IsNull ? TranslationUnit.GetOrCreate<TypedefNameDecl>(Handle.TypedefNameForAnonDecl) : null);
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

    public IReadOnlyList<IReadOnlyList<NamedDecl>> TemplateParameterLists => _templateParameterLists.Value;

    public TypedefNameDecl? TypedefNameForAnonDecl => _typedefNameForAnonDecl.Value;
}
