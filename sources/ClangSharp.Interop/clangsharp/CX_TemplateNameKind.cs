// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.
// Ported from https://github.com/dotnet/clangsharp/blob/main/sources/libClangSharp

namespace ClangSharp.Interop;

public enum CX_TemplateNameKind
{
    CX_TNK_Invalid,
    CX_TNK_Template,
    CX_TNK_OverloadedTemplate,
    CX_TNK_AssumedTemplate,
    CX_TNK_QualifiedTemplate,
    CX_TNK_DependentTemplate,
    CX_TNK_SubstTemplateTemplateParm,
    CX_TNK_SubstTemplateTemplateParmPack,
    CX_TNK_UsingTemplate,
    CX_TNK_DeducedTemplate,
}
