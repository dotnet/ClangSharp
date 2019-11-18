// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class PointerType : Type
    {
        private readonly Lazy<Type> _pointeeType;

        internal PointerType(CXType handle) : base(handle, CXTypeKind.CXType_Pointer, CX_TypeClass.CX_TypeClass_Pointer)
        {
            _pointeeType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.PointeeType));
        }

        public Type PointeeType => _pointeeType.Value;
    }
}
