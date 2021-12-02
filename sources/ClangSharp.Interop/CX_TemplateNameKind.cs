// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.
// Ported from https://github.com/dotnet/clangsharp/blob/main/sources/libClangSharp

namespace ClangSharp.Interop
{
    public enum CX_TemplateNameKind
    {
        CX_TNK_Invalid,
        CX_TNK_Template = 1,
        CX_TNK_OverloadedTemplate = 2,
        CX_TNK_AssumedTemplate = 3,
        CX_TNK_QualifiedTemplate = 4,
        CX_TNK_DependentTemplate = 5,
        CX_TNK_SubstTemplateTemplateParm = 6,
        CX_TNK_SubstTemplateTemplateParmPack = 7
    }
}
