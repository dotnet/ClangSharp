// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public sealed class RecordType : TagType
{

    internal RecordType(CXType handle) : base(handle, CXType_Record, CX_TypeClass_Record)
    {
    }

    public new RecordDecl Decl => (RecordDecl)base.Decl;
}
