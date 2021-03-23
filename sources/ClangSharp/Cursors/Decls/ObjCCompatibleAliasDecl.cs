// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ObjCCompatibleAliasDecl : NamedDecl
    {
        private readonly Lazy<ObjCInterfaceDecl> _classInterface;

        internal ObjCCompatibleAliasDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnexposedDecl, CX_DeclKind.CX_DeclKind_ObjCCompatibleAlias)
        {

            _classInterface = new Lazy<ObjCInterfaceDecl>(() => TranslationUnit.GetOrCreate<ObjCInterfaceDecl>(Handle.GetSubDecl(0)));
        }

        public ObjCInterfaceDecl ClassInterface => _classInterface.Value;
    }
}
