// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class MemberPointerType : Type
    {
        internal MemberPointerType(CXType handle) : base(handle, CXTypeKind.CXType_MemberPointer, CX_TypeClass.CX_TypeClass_MemberPointer)
        {
        }
    }
}
