// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class InheritableAttr : Attr
    {
        internal InheritableAttr(CXCursor handle) : base(handle)
        {
            if ((CX_AttrKind.CX_AttrKind_LastInheritableAttr < handle.AttrKind) || (handle.AttrKind < CX_AttrKind.CX_AttrKind_FirstInheritableAttr))
            {
                throw new ArgumentException(nameof(handle));
            }
        }
    }
}
