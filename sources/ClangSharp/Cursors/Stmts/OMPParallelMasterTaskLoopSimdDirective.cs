// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class OMPParallelMasterTaskLoopSimdDirective : OMPLoopDirective
    {
        internal OMPParallelMasterTaskLoopSimdDirective(CXCursor handle) : base(handle, CXCursorKind.CXCursor_OMPParallelMasterTaskLoopSimdDirective, CX_StmtClass.CX_StmtClass_OMPParallelMasterTaskLoopSimdDirective)
        {
        }
    }
}
