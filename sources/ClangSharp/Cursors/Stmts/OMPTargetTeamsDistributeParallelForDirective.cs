// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class OMPTargetTeamsDistributeParallelForDirective : OMPLoopDirective
    {
        internal OMPTargetTeamsDistributeParallelForDirective(CXCursor handle) : base(handle, CXCursorKind.CXCursor_OMPTargetTeamsDistributeParallelForDirective, CX_StmtClass.CX_StmtClass_OMPTargetTeamsDistributeParallelForDirective)
        {
        }
    }
}
