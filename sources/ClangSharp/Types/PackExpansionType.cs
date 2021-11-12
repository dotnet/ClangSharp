// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class PackExpansionType : Type
    {
        private readonly Lazy<Type> _pattern;

        internal PackExpansionType(CXType handle) : base(handle, CXTypeKind.CXType_Unexposed, CX_TypeClass.CX_TypeClass_PackExpansion)
        {
            _pattern = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.OriginalType));
        }

        public Type Pattern => _pattern.Value;
    }
}
