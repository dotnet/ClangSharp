// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class DecayedType : AdjustedType
    {
        private readonly Lazy<Type> _decayedType;

        internal DecayedType(CXType handle) : base(handle, CXTypeKind.CXType_Unexposed, CX_TypeClass.CX_TypeClass_Decayed)
        {
            _decayedType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.DecayedType));
        }

        public Type GetDecayedType => _decayedType.Value;
    }
}
