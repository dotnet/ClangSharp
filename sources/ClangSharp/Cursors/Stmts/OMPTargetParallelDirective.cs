// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class OMPTargetParallelDirective : OMPExecutableDirective
    {
        internal OMPTargetParallelDirective(CXCursor handle) : base(handle, CXCursorKind.CXCursor_OMPTargetParallelDirective, CX_StmtClass.CX_StmtClass_OMPTargetParallelDirective)
        {
        }
    }
}
