// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;

namespace ClangSharp;

public class TypeWithKeyword : Type
{
    private protected TypeWithKeyword(CXType handle, CXTypeKind expectedTypeKind, CX_TypeClass expectedTypeClass, params ReadOnlySpan<CXTypeKind> additionalExpectedKinds) : base(handle, expectedTypeKind, expectedTypeClass, additionalExpectedKinds)
    {
    }
}
