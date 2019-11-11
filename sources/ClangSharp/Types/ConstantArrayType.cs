// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ConstantArrayType : ArrayType
    {
        internal ConstantArrayType(CXType handle) : base(handle, CXTypeKind.CXType_ConstantArray)
        {
        }

        public long Size => Handle.ArraySize;
    }
}
