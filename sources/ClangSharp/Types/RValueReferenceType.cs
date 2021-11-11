// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class RValueReferenceType : ReferenceType
    {
        internal RValueReferenceType(CXType handle) : base(handle, CXTypeKind.CXType_RValueReference, CX_TypeClass.CX_TypeClass_RValueReference)
        {
        }
    }
}
