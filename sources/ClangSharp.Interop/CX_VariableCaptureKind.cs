// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.
// Ported from https://github.com/microsoft/ClangSharp/blob/main/sources/libClangSharp

namespace ClangSharp.Interop
{
    public enum CX_VariableCaptureKind
    {
        CX_VCK_Invalid,
        CX_VCK_This,
        CX_VCK_ByRef,
        CX_VCK_ByCopy,
        CX_VCK_VLAType
    }
}
