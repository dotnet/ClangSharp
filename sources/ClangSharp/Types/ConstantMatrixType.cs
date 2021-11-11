// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ConstantMatrixType : MatrixType
    {
        internal ConstantMatrixType(CXType handle) : base(handle, CXTypeKind.CXType_Unexposed, CX_TypeClass.CX_TypeClass_ConstantMatrix)
        {
        }

        public uint NumColumns => unchecked((uint)Handle.NumColumns);

        public uint NumElementsFlattened => unchecked((uint)Handle.NumElementsFlattened);

        public uint NumRows => unchecked((uint)Handle.NumRows);
    }
}
