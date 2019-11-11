// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ElaboratedType : TypeWithKeyword
    {
        private readonly Lazy<Type> _namedType;

        internal ElaboratedType(CXType handle) : base(handle, CXTypeKind.CXType_Elaborated)
        {
            _namedType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.NamedType));
        }

        public Type NamedType => _namedType.Value;
    }
}
