// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ObjCTypeParamType : Type
    {
        internal ObjCTypeParamType(CXType handle) : base(handle, CXTypeKind.CXType_ObjCTypeParam, CX_TypeClass.CX_TypeClass_ObjCTypeParam)
        {
        }
    }
}
