// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class StmtAttr : Attr
    {
        internal StmtAttr(CXCursor handle) : base(handle)
        {
            if ((CX_AttrKind.CX_AttrKind_LastStmtAttr < handle.AttrKind) || (handle.AttrKind < CX_AttrKind.CX_AttrKind_FirstStmtAttr))
            {
                throw new ArgumentException(nameof(handle));
            }
        }
    }
}
