// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class RecordType : TagType
    {
        private readonly Lazy<Type> _desugaredType;

        internal RecordType(CXType handle) : base(handle, CXTypeKind.CXType_Record, CX_TypeClass.CX_TypeClass_Record)
        {
            _desugaredType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.Desugar()));
        }

        public new RecordDecl Decl => (RecordDecl)base.Decl;

        public bool IsSugared => Handle.IsSugared;

        public Type Desugar() => _desugaredType.Value;
    }
}
