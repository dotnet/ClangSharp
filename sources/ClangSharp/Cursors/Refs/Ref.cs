// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class Ref : Cursor
    {
        private readonly Lazy<NamedDecl> _referenced;
        private readonly Lazy<Type> _type;

        private protected Ref(CXCursor handle, CXCursorKind expectedCursorKind) : base(handle, expectedCursorKind)
        {
            _referenced = new Lazy<NamedDecl>(() => TranslationUnit.GetOrCreate<NamedDecl>(Handle.Referenced));
            _type = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.Type));
        }

        public NamedDecl Referenced => _referenced.Value;

        public Type Type => _type.Value;

        internal static new Ref Create(CXCursor handle)
        {
            Ref result;

            switch (handle.Kind)
            {
                case CXCursorKind.CXCursor_CXXBaseSpecifier:
                {
                    result = new CXXBaseSpecifier(handle);
                    break;
                }

                default:
                {
                    Debug.WriteLine($"Unhandled reference kind: {handle.KindSpelling}.");
                    result = new Ref(handle, handle.Kind);
                    break;
                }
            }

            return result;
        }
    }
}
