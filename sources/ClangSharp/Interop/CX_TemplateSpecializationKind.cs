// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.
// Ported from https://github.com/microsoft/ClangSharp/blob/master/sources/libClangSharp

namespace ClangSharp.Interop
{
    public enum CX_TemplateSpecializationKind
    {
        CX_TSK_Invalid,
        CX_TSK_Undeclared,
        CX_TSK_ImplicitInstantiation,
        CX_TSK_ExplicitSpecialization,
        CX_TSK_ExplicitInstantiationDeclaration,
        CX_TSK_ExplicitInstantiationDefinition,
    }
}
