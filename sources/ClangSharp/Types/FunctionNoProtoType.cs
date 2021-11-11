// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class FunctionNoProtoType : FunctionType
    {

        internal FunctionNoProtoType(CXType handle) : base(handle, CXTypeKind.CXType_FunctionNoProto, CX_TypeClass.CX_TypeClass_FunctionNoProto)
        {
        }
    }
}
