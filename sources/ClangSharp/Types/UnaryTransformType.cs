// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using ClangSharp.Interop;

namespace ClangSharp
{
    public class UnaryTransformType : Type
    {
        internal UnaryTransformType(CXType handle) : this(handle, CXTypeKind.CXType_Unexposed, CX_TypeClass.CX_TypeClass_UnaryTransform)
        {
        }

        private protected UnaryTransformType(CXType handle, CXTypeKind expectedTypeKind, CX_TypeClass expectedTypeClass) : base(handle, expectedTypeKind, expectedTypeClass)
        {
        }
    }
}
