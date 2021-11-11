// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class AtomicType : Type
    {
        private readonly Lazy<Type> _valueType;

        internal AtomicType(CXType handle) : base(handle, CXTypeKind.CXType_Atomic, CX_TypeClass.CX_TypeClass_Atomic)
        {
            _valueType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.ValueType));
        }

        public Type ValueType => _valueType.Value;
    }
}
