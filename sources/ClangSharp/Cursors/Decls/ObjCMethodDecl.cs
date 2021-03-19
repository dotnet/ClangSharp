// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ObjCMethodDecl : NamedDecl, IDeclContext
    {
        private readonly Lazy<IReadOnlyList<Decl>> _decls;

        internal ObjCMethodDecl(CXCursor handle) : base(handle, handle.Kind, CX_DeclKind.CX_DeclKind_ObjCMethod)
        {
            if ((handle.Kind != CXCursorKind.CXCursor_ObjCInstanceMethodDecl) && (handle.Kind != CXCursorKind.CXCursor_ObjCClassMethodDecl))
            {
                throw new ArgumentException(nameof(handle));
            }

            _decls = new Lazy<IReadOnlyList<Decl>>(() => CursorChildren.OfType<Decl>().ToList());
        }

        public IReadOnlyList<Decl> Decls => _decls.Value;

        public bool IsClassMethod() => CursorKind == CXCursorKind.CXCursor_ObjCClassMethodDecl;

        public bool IsInstanceMethod() => CursorKind == CXCursorKind.CXCursor_ObjCInstanceMethodDecl;
    }
}
