// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public sealed class HLSLInlineSpirvType : Type
{
    internal HLSLInlineSpirvType(CXType handle) : base(handle, CXType_HLSLInlineSpirv, CX_TypeClass_HLSLInlineSpirv)
    {
    }
}
