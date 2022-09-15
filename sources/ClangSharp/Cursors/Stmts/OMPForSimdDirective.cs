// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;

namespace ClangSharp;

public sealed class OMPForSimdDirective : OMPLoopDirective
{
    internal OMPForSimdDirective(CXCursor handle) : base(handle, CXCursorKind.CXCursor_OMPForSimdDirective, CX_StmtClass.CX_StmtClass_OMPForSimdDirective)
    {
    }
}
