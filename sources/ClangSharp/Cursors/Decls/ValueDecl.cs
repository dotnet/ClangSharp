// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class ValueDecl : NamedDecl
    {
        private readonly Lazy<Type> _type;

        private protected ValueDecl(CXCursor handle, CXCursorKind expectedKind) : base(handle, expectedKind)
        {
            _type = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.Type));
        }

        public Type Type => _type.Value;
    }
}
