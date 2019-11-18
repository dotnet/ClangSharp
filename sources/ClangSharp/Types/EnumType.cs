// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class EnumType : TagType
    {
        internal EnumType(CXType handle) : base(handle, CXTypeKind.CXType_Enum, CX_TypeClass.CX_TypeClass_Enum)
        {
        }
    }
}
