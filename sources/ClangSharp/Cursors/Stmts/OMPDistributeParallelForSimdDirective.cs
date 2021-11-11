// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class OMPDistributeParallelForSimdDirective : OMPLoopDirective
    {
        internal OMPDistributeParallelForSimdDirective(CXCursor handle) : base(handle, CXCursorKind.CXCursor_OMPDistributeParallelForSimdDirective, CX_StmtClass.CX_StmtClass_OMPDistributeParallelForSimdDirective)
        {
        }
    }
}
