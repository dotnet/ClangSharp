// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public sealed class SubstTemplateTypeParmPackType : Type
{
    private readonly ValueLazy<TemplateArgument> _argumentPack;
    private readonly ValueLazy<TemplateTypeParmType> _replacedParameter;

    internal SubstTemplateTypeParmPackType(CXType handle) : base(handle, CXType_Unexposed, CX_TypeClass_SubstTemplateTypeParmPack)
    {
        _argumentPack = new ValueLazy<TemplateArgument>(() => TranslationUnit.GetOrCreate(Handle.GetTemplateArgument(0)));
        _replacedParameter = new ValueLazy<TemplateTypeParmType>(() => TranslationUnit.GetOrCreate<TemplateTypeParmType>(Handle.OriginalType));
    }

    public TemplateArgument ArgumentPack => _argumentPack.Value;

    public TemplateTypeParmType ReplacedParameter => _replacedParameter.Value;
}
