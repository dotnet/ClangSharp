// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.
// Ported from https://github.com/microsoft/ClangSharp/blob/main/sources/libClangSharp

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
