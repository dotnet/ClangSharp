// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public sealed class SubstTemplateTypeParmPackType : Type
{
    private ValueLazy<SubstTemplateTypeParmPackType, TemplateArgument> _argumentPack;
    private ValueLazy<SubstTemplateTypeParmPackType, TemplateTypeParmType> _replacedParameter;

    internal unsafe SubstTemplateTypeParmPackType(CXType handle) : base(handle, CXType_Unexposed, CX_TypeClass_SubstTemplateTypeParmPack)
    {
        _argumentPack = new ValueLazy<SubstTemplateTypeParmPackType, TemplateArgument>(&ArgumentPackFactory);
        _replacedParameter = new ValueLazy<SubstTemplateTypeParmPackType, TemplateTypeParmType>(&ReplacedParameterFactory);
    }

    public TemplateArgument ArgumentPack => _argumentPack.GetValue(this);

    public TemplateTypeParmType ReplacedParameter => _replacedParameter.GetValue(this);

    private static unsafe TemplateTypeParmType ReplacedParameterFactory(SubstTemplateTypeParmPackType self) => self.TranslationUnit.GetOrCreate<TemplateTypeParmType>(self.Handle.OriginalType);

    private static unsafe TemplateArgument ArgumentPackFactory(SubstTemplateTypeParmPackType self) => self.TranslationUnit.GetOrCreate(self.Handle.GetTemplateArgument(0));
}
