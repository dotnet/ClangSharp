// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

namespace ClangSharp.Interop
{
    public enum CXIdxEntityCXXTemplateKind
    {
        CXIdxEntity_NonTemplate = 0,
        CXIdxEntity_Template = 1,
        CXIdxEntity_TemplatePartialSpecialization = 2,
        CXIdxEntity_TemplateSpecialization = 3,
    }
}
