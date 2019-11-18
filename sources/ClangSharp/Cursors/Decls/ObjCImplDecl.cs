// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class ObjCImplDecl : ObjCContainerDecl
    {
        private protected ObjCImplDecl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind, expectedDeclKind)
        {
            if ((CX_DeclKind.CX_DeclKind_LastObjCImpl < handle.DeclKind) || (handle.DeclKind < CX_DeclKind.CX_DeclKind_FirstObjCImpl))
            {
                throw new ArgumentException(nameof(handle));
            }
        }
    }
}
