// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class PipeType : Type
    {
        private readonly Lazy<Type> _elementType;

        internal PipeType(CXType handle) : base(handle, CXTypeKind.CXType_Pipe, CX_TypeClass.CX_TypeClass_Pipe)
        {
            _elementType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.ElementType));
        }

        public Type ElementType => _elementType.Value;
    }
}
