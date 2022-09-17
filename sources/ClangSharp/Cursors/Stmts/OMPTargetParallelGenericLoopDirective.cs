// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;

namespace ClangSharp;

public sealed class OMPTargetParallelGenericLoopDirective : OMPLoopDirective
{
    internal OMPTargetParallelGenericLoopDirective(CXCursor handle) : base(handle, CXCursorKind.CXCursor_OMPTargetParallelGenericLoopDirective, CX_StmtClass.CX_StmtClass_OMPTargetParallelGenericLoopDirective)
    {
    }
}
