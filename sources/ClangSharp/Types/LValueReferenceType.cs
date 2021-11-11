// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class LValueReferenceType : ReferenceType
    {

        internal LValueReferenceType(CXType handle) : base(handle, CXTypeKind.CXType_LValueReference, CX_TypeClass.CX_TypeClass_LValueReference)
        {
        }
    }
}
