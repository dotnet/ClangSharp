// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class OMPUnrollDirective : OMPLoopTransformationDirective
    {
        internal OMPUnrollDirective(CXCursor handle) : base(handle, CXCursorKind.CXCursor_OMPUnrollDirective, CX_StmtClass.CX_StmtClass_OMPUnrollDirective)
        {
        }
    }
}
