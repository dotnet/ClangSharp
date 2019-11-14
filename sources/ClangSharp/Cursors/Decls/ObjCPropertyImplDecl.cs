// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ObjCPropertyImplDecl : Decl
    {
        internal ObjCPropertyImplDecl(CXCursor handle) : base(handle, handle.Kind, CX_DeclKind.CX_DeclKind_ObjCPropertyImpl)
        {
            if ((handle.Kind != CXCursorKind.CXCursor_ObjCSynthesizeDecl) && (handle.Kind != CXCursorKind.CXCursor_ObjCDynamicDecl))
            {
                throw new ArgumentException(nameof(handle));
            }
        }
    }
}
