// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public sealed class SubstTemplateTypeParmType : Type
{
    private readonly Lazy<Decl?> _associatedDecl;
    private readonly Lazy<TemplateTypeParmType> _replacedParameter;

    internal SubstTemplateTypeParmType(CXType handle) : base(handle, CXType_Unexposed, CX_TypeClass_SubstTemplateTypeParm)
    {
        _associatedDecl = new Lazy<Decl?>(() => {
            CXCursor cursor = clangsharp.Type_getSubstTemplateTypeParamAssociatedDecl(Handle);
            return cursor.IsNull ? null : TranslationUnit.GetOrCreate<Decl>(cursor);
        });
        _replacedParameter = new Lazy<TemplateTypeParmType>(() => TranslationUnit.GetOrCreate<TemplateTypeParmType>(Handle.OriginalType));
    }

    public TemplateTypeParmType ReplacedParameter => _replacedParameter.Value;

    public Type ReplacementType => Desugar;

    public Decl? AssociatedDecl => _associatedDecl.Value;
}
