// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class CXXBaseSpecifier : Ref
    {
        internal CXXBaseSpecifier(CXCursor handle) : base(handle, CXCursorKind.CXCursor_CXXBaseSpecifier)
        {
        }

        public CX_CXXAccessSpecifier AccessSpecifier => Handle.CXXAccessSpecifier;

        public bool IsVirtual => Handle.IsVirtualBase;
    }
}
