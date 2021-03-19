// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ExtIntType : Type
    {

        internal ExtIntType(CXType handle) : base(handle, CXTypeKind.CXType_Unexposed, CX_TypeClass.CX_TypeClass_ExtInt)
        {
        }

        public bool IsSigned => Handle.IsSigned;

        public bool IsUnsigned => Handle.IsUnsigned;

        public uint NumBits => unchecked((uint)Handle.NumBits);
    }
}
