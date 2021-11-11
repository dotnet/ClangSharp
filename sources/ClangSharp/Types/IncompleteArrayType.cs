// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class IncompleteArrayType : ArrayType
    {

        internal IncompleteArrayType(CXType handle) : base(handle, CXTypeKind.CXType_IncompleteArray, CX_TypeClass.CX_TypeClass_IncompleteArray)
        {
        }
    }
}
