// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class StmtAttr : Attr
    {
        internal StmtAttr(CXCursor handle) : base(handle)
        {
            if (handle.AttrKind is > CX_AttrKind.CX_AttrKind_LastStmtAttr or < CX_AttrKind.CX_AttrKind_FirstStmtAttr)
            {
                throw new ArgumentOutOfRangeException(nameof(handle));
            }
        }
    }
}
