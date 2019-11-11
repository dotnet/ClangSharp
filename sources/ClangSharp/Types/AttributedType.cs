// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class AttributedType : Type
    {
        private readonly Lazy<Type> _modifiedType;

        internal AttributedType(CXType handle) : base(handle, CXTypeKind.CXType_Attributed)
        {
            _modifiedType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.ModifiedType));
        }

        public Type ModifiedType => _modifiedType.Value;
    }
}
