// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ObjCMethodDecl : NamedDecl, IDeclContext
    {
        internal ObjCMethodDecl(CXCursor handle) : base(handle, handle.Kind, CX_DeclKind.CX_DeclKind_ObjCMethod)
        {
            if ((handle.Kind != CXCursorKind.CXCursor_ObjCInstanceMethodDecl) && (handle.Kind != CXCursorKind.CXCursor_ObjCClassMethodDecl))
            {
                throw new ArgumentException(nameof(handle));
            }
        }

        public new ObjCMethodDecl CanonicalDecl => (ObjCMethodDecl)base.CanonicalDecl;

        public bool IsClassMethod() => CursorKind == CXCursorKind.CXCursor_ObjCClassMethodDecl;

        public bool IsInstanceMethod() => CursorKind == CXCursorKind.CXCursor_ObjCInstanceMethodDecl;
    }
}
