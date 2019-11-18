// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using ClangSharp.Interop;

namespace ClangSharp
{
    public class ObjCObjectType : Type
    {
        internal ObjCObjectType(CXType handle) : this(handle, CXTypeKind.CXType_ObjCObject, CX_TypeClass.CX_TypeClass_ObjCObject)
        {
        }

        private protected ObjCObjectType(CXType handle, CXTypeKind expectedTypeKind, CX_TypeClass expectedTypeClass) : base(handle, expectedTypeKind, expectedTypeClass)
        {
        }
    }
}
