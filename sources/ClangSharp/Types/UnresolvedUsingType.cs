// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class UnresolvedUsingType : Type
    {
        private readonly Lazy<UnresolvedUsingTypenameDecl> _decl;

        internal UnresolvedUsingType(CXType handle) : base(handle, CXTypeKind.CXType_Unexposed, CX_TypeClass.CX_TypeClass_UnresolvedUsing)
        {
            _decl = new Lazy<UnresolvedUsingTypenameDecl>(() => TranslationUnit.GetOrCreate<UnresolvedUsingTypenameDecl>(Handle.Declaration));
        }

        public UnresolvedUsingTypenameDecl Decl => _decl.Value;
    }
}
