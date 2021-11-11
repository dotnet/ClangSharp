// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class RecordType : TagType
    {

        internal RecordType(CXType handle) : base(handle, CXTypeKind.CXType_Record, CX_TypeClass.CX_TypeClass_Record)
        {
        }

        public new RecordDecl Decl => (RecordDecl)base.Decl;
    }
}
