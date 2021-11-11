// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class TypeDecl : NamedDecl
    {
        private readonly Lazy<Type> _typeForDecl;

        private protected TypeDecl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind, expectedDeclKind)
        {
            if (handle.DeclKind is > CX_DeclKind.CX_DeclKind_LastType or < CX_DeclKind.CX_DeclKind_FirstType)
            {
                throw new ArgumentOutOfRangeException(nameof(handle));
            }

            _typeForDecl = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.Type));
        }

        public Type TypeForDecl => _typeForDecl.Value;
    }
}
