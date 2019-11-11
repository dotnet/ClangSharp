// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class EnumDecl : TagDecl
    {
        private readonly Lazy<IReadOnlyList<EnumConstantDecl>> _enumerators;
        private readonly Lazy<Type> _integerType;

        internal EnumDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_EnumDecl)
        {
            _enumerators = new Lazy<IReadOnlyList<EnumConstantDecl>>(() => Decls.OfType<EnumConstantDecl>().ToList());
            _integerType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.EnumDecl_IntegerType));
        }

        public IReadOnlyList<EnumConstantDecl> Enumerators => _enumerators.Value;

        public Type IntegerType => _integerType.Value;

        public bool IsScoped => Handle.EnumDecl_IsScoped;
    }
}
