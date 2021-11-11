// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class TagType : Type
    {
        private readonly Lazy<TagDecl> _decl;

        private protected TagType(CXType handle, CXTypeKind expectedTypeKind, CX_TypeClass expectedTypeClass) : base(handle, expectedTypeKind, expectedTypeClass)
        {
            _decl = new Lazy<TagDecl>(() => TranslationUnit.GetOrCreate<TagDecl>(Handle.Declaration));
        }

        public TagDecl Decl => _decl.Value;
    }
}
