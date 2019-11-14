// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class TypeAttr : Attr
    {
        internal TypeAttr(CXCursor handle) : base(handle)
        {
            if ((CX_AttrKind.CX_AttrKind_LastTypeAttr < handle.AttrKind) || (handle.AttrKind < CX_AttrKind.CX_AttrKind_FirstTypeAttr))
            {
                throw new ArgumentException(nameof(handle));
            }
        }
    }
}
