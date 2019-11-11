// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Collections.Generic;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class FunctionProtoType : FunctionType
    {
        private Type[] _paramTypes;

        internal FunctionProtoType(CXType handle) : base(handle, CXTypeKind.CXType_FunctionProto)
        {
        }

        public CXCursor_ExceptionSpecificationKind ExceptionSpecType => Handle.ExceptionSpecificationType;

        public bool IsVariadic => Handle.IsFunctionTypeVariadic;

        public uint NumParams => (uint)Handle.NumArgTypes;

        public IReadOnlyList<Type> ParamTypes
        {
            get
            {
                if (_paramTypes is null)
                {
                    uint numParams = NumParams;
                    _paramTypes = new Type[numParams];

                    for (var index = 0u; index < numParams; index++)
                    {
                        var paramType = Handle.GetArgType(index);
                        _paramTypes[index] = TranslationUnit.GetOrCreate<Type>(paramType);
                    }
                }
                return _paramTypes;
            }
        }

        public CXRefQualifierKind RefQualifier => Handle.CXXRefQualifier;
    }
}
