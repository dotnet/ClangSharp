// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class TypedefType : Type
    {
        private readonly Lazy<TypedefNameDecl> _decl;

        internal TypedefType(CXType handle) : base(handle, CXTypeKind.CXType_Typedef)
        {
            _decl = new Lazy<TypedefNameDecl>(() => TranslationUnit.GetOrCreate<TypedefNameDecl>(Handle.Declaration));
        }

        public TypedefNameDecl Decl => _decl.Value;
    }
}
