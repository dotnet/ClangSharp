// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using ClangSharp.Interop;

namespace ClangSharp
{
    public class VectorType : Type
    {
        internal VectorType(CXType handle) : base(handle, CXTypeKind.CXType_Vector, CX_TypeClass.CX_TypeClass_Vector)
        {
        }

        private protected VectorType(CXType handle, CXTypeKind expectedTypeKind, CX_TypeClass expectedTypeClass) : base(handle, expectedTypeKind, expectedTypeClass)
        {
        }
    }
}
