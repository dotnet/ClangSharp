// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ObjCIvarDecl : FieldDecl
    {
        private readonly Lazy<ObjCInterfaceDecl> _containingInterface;
        private readonly Lazy<ObjCIvarDecl> _nextIvar;

        internal ObjCIvarDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_ObjCIvarDecl, CX_DeclKind.CX_DeclKind_ObjCIvar)
        {
            _containingInterface = new Lazy<ObjCInterfaceDecl>(() => TranslationUnit.GetOrCreate<ObjCInterfaceDecl>(Handle.GetSubDecl(0)));
            _nextIvar = new Lazy<ObjCIvarDecl>(() => TranslationUnit.GetOrCreate<ObjCIvarDecl>(Handle.GetSubDecl(1)));
        }

        public ObjCInterfaceDecl ContainingInterface => _containingInterface.Value;

        public ObjCIvarDecl NextIvar => _nextIvar.Value;
    }
}
