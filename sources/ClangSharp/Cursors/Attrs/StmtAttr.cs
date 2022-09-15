// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;

namespace ClangSharp;

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
