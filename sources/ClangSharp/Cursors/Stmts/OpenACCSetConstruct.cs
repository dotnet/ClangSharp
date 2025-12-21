// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class OpenACCSetConstruct : OpenACCConstructStmt
{
    internal OpenACCSetConstruct(CXCursor handle) : base(handle, CXCursor_OpenACCSetConstruct, CX_StmtClass_OpenACCSetConstruct)
    {
    }
}
