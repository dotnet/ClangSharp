// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ObjCProtocolDecl : ObjCContainerDecl, IRedeclarable<ObjCProtocolDecl>
    {
        internal ObjCProtocolDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_ObjCProtocolDecl, CX_DeclKind.CX_DeclKind_ObjCProtocol)
        {
        }
    }
}
