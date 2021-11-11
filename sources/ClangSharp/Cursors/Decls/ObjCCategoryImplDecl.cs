// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ObjCCategoryImplDecl : ObjCImplDecl
    {
        private readonly Lazy<ObjCCategoryDecl> _categoryDecl;

        internal ObjCCategoryImplDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_ObjCCategoryImplDecl, CX_DeclKind.CX_DeclKind_ObjCCategoryImpl)
        {
            _categoryDecl = new Lazy<ObjCCategoryDecl>(() => TranslationUnit.GetOrCreate<ObjCCategoryDecl>(Handle.GetSubDecl(1)));
        }

        public ObjCCategoryDecl CategoryDecl => _categoryDecl.Value;
    }
}
