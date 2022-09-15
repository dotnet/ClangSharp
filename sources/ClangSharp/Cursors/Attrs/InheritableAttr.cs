// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;

namespace ClangSharp;

public class InheritableAttr : Attr
{
    internal InheritableAttr(CXCursor handle) : base(handle)
    {
        if (handle.AttrKind is > CX_AttrKind.CX_AttrKind_LastInheritableAttr or < CX_AttrKind.CX_AttrKind_FirstInheritableAttr)
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
        }
    }
}
