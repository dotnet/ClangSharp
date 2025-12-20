// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class OpenACCShutdownConstruct : OpenACCConstructStmt
{
    internal OpenACCShutdownConstruct(CXCursor handle) : base(handle, CXCursor_OpenACCShutdownConstruct, CX_StmtClass_OpenACCShutdownConstruct)
    {
    }
}
