// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ObjCInterfaceType : ObjCObjectType
    {
        private readonly Lazy<ObjCInterfaceDecl> _decl;

        internal ObjCInterfaceType(CXType handle) : base(handle, CXTypeKind.CXType_ObjCObject, CX_TypeClass.CX_TypeClass_ObjCInterface)
        {
            _decl = new Lazy<ObjCInterfaceDecl>(() => TranslationUnit.GetOrCreate<ObjCInterfaceDecl>(Handle.Declaration));
        }

        public ObjCInterfaceDecl Decl => _decl.Value;
    }
}
