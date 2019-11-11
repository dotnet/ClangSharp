// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using ClangSharp.Interop;

namespace ClangSharp
{
    public class CXXMethodDecl : FunctionDecl
    {
        internal CXXMethodDecl(CXCursor handle) : this(handle, CXCursorKind.CXCursor_CXXMethod)
        {
        }

        private protected CXXMethodDecl(CXCursor handle, CXCursorKind expectedKind) : base(handle, expectedKind)
        {
        }

        public bool IsConst => Handle.CXXMethod_IsConst;

        public bool IsDefaulted => Handle.CXXMethod_IsDefaulted;

        public bool IsStatic => Handle.CXXMethod_IsStatic;

        public bool IsVirtual => Handle.CXXMethod_IsVirtual;
    }
}
