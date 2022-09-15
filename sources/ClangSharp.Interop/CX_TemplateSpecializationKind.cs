// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.
// Ported from https://github.com/dotnet/clangsharp/blob/main/sources/libClangSharp

namespace ClangSharp.Interop;

public enum CX_TemplateSpecializationKind
{
    CX_TSK_Invalid,
    CX_TSK_Undeclared,
    CX_TSK_ImplicitInstantiation,
    CX_TSK_ExplicitSpecialization,
    CX_TSK_ExplicitInstantiationDeclaration,
    CX_TSK_ExplicitInstantiationDefinition,
}
