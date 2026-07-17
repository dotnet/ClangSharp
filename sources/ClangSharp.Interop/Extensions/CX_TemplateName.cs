// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.
// Ported from https://github.com/dotnet/clangsharp/blob/main/sources/libClangSharp

namespace ClangSharp.Interop;

public unsafe partial struct CX_TemplateName
{
    public readonly CXCursor AsTemplateDecl => clangsharp.TemplateName_getAsTemplateDecl(this);

    public readonly bool ContainsUnexpandedParameterPack => clangsharp.TemplateName_getContainsUnexpandedParameterPack(this) != 0;

    public readonly bool IsDependent => clangsharp.TemplateName_getIsDependent(this) != 0;

    public readonly bool IsInstantiationDependent => clangsharp.TemplateName_getIsInstantiationDependent(this) != 0;

    public readonly CX_TemplateName Underlying => clangsharp.TemplateName_getUnderlying(this);
}
