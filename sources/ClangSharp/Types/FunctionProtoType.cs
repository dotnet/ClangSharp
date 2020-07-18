// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class FunctionProtoType : FunctionType
    {
        private readonly Lazy<Type> _desugaredType;
        private readonly Lazy<IReadOnlyList<Type>> _paramTypes;

        internal FunctionProtoType(CXType handle) : base(handle, CXTypeKind.CXType_FunctionProto, CX_TypeClass.CX_TypeClass_FunctionProto)
        {
            _desugaredType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.Desugar()));
            _paramTypes = new Lazy<IReadOnlyList<Type>>(() => {
                var paramTypeCount = Handle.NumArgTypes;
                var paramTypes = new List<Type>(paramTypeCount);

                for (int i = 0; i < paramTypeCount; i++)
                {
                    var paramType = TranslationUnit.GetOrCreate<Type>(Handle.GetArgType(unchecked((uint)i)));
                    paramTypes.Add(paramType);
                }

                return paramTypes;
            });
        }

        public CXCursor_ExceptionSpecificationKind ExceptionSpecType => Handle.ExceptionSpecificationType;

        public bool IsSugared => Handle.IsSugared;

        public bool IsVariadic => Handle.IsFunctionTypeVariadic;

        public uint NumParams => (uint)Handle.NumArgTypes;

        public IReadOnlyList<Type> ParamTypes => _paramTypes.Value;

        public CXRefQualifierKind RefQualifier => Handle.CXXRefQualifier;

        public Type Desugar() => _desugaredType.Value;
    }
}
