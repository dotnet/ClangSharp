// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class TypedefNameDecl : TypeDecl, IRedeclarable<TypedefNameDecl>
    {
        private readonly Lazy<Type> _underlyingType;

        private protected TypedefNameDecl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind, expectedDeclKind)
        {
            if (handle.DeclKind is > CX_DeclKind.CX_DeclKind_LastTypedefName or < CX_DeclKind.CX_DeclKind_FirstTypedefName)
            {
                throw new ArgumentOutOfRangeException(nameof(handle));
            }

            _underlyingType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.TypedefDeclUnderlyingType));
        }

        public new TypedefNameDecl CanonicalDecl => (TypedefNameDecl)base.CanonicalDecl;

        public bool IsTransparentTag => Handle.IsTransparent;

        public Type UnderlyingType => _underlyingType.Value;
    }
}
