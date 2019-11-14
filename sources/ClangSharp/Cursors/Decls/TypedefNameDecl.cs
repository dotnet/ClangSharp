// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class TypedefNameDecl : TypeDecl, IRedeclarable<TypedefNameDecl>
    {
        private readonly Lazy<Type> _underlyingType;

        private protected TypedefNameDecl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind, expectedDeclKind)
        {
            if ((CX_DeclKind.CX_DeclKind_LastTypedefName < handle.DeclKind) || (handle.DeclKind < CX_DeclKind.CX_DeclKind_FirstTypedefName))
            {
                throw new ArgumentException(nameof(handle));
            }

            _underlyingType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.TypedefDeclUnderlyingType));
        }

        public Type UnderlyingType => _underlyingType.Value;
    }
}
