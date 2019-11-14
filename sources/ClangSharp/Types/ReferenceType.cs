// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class ReferenceType : Type
    {
        private readonly Lazy<Type> _pointeeType;

        private protected ReferenceType(CXType handle, CXTypeKind expectedTypeKind, CX_TypeClass expectedTypeClass) : base(handle, expectedTypeKind, expectedTypeClass)
        {
            _pointeeType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.PointeeType));
        }

        public Type PointeeType => _pointeeType.Value;
    }
}
