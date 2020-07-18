// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class DecayedType : AdjustedType
    {
        private readonly Lazy<Type> _decayedType;
        private readonly Lazy<Type> _pointeeType;

        internal DecayedType(CXType handle) : base(handle, CXTypeKind.CXType_Unexposed, CX_TypeClass.CX_TypeClass_Decayed)
        {
            _decayedType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.DecayedType));
            _pointeeType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.PointeeType));
        }

        public Type GetDecayedType => _decayedType.Value;

        public Type PointeeType => _pointeeType.Value;
    }
}
