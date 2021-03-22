// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ObjCTypeParamType : Type
    {
        private readonly Lazy<ObjCTypeParamDecl> _decl;

        internal ObjCTypeParamType(CXType handle) : base(handle, CXTypeKind.CXType_ObjCTypeParam, CX_TypeClass.CX_TypeClass_ObjCTypeParam)
        {
            _decl = new Lazy<ObjCTypeParamDecl>(() => TranslationUnit.GetOrCreate<ObjCTypeParamDecl>(Handle.Declaration));
        }

        public ObjCTypeParamDecl Decl => _decl.Value;
    }
}
