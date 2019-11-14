// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class CXXMethodDecl : FunctionDecl
    {
        internal CXXMethodDecl(CXCursor handle) : this(handle, CXCursorKind.CXCursor_CXXMethod, CX_DeclKind.CX_DeclKind_CXXMethod)
        {
        }

        private protected CXXMethodDecl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind, expectedDeclKind)
        {
            if ((CX_DeclKind.CX_DeclKind_LastCXXMethod < handle.DeclKind) || (handle.DeclKind < CX_DeclKind.CX_DeclKind_FirstCXXMethod))
            {
                throw new ArgumentException(nameof(handle));
            }
        }

        public bool IsConst => Handle.CXXMethod_IsConst;

        public bool IsDefaulted => Handle.CXXMethod_IsDefaulted;

        public bool IsStatic => Handle.CXXMethod_IsStatic;

        public bool IsVirtual => Handle.CXXMethod_IsVirtual;
    }
}
