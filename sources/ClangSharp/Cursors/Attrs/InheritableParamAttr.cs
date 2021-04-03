// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class InheritableParamAttr : InheritableAttr
    {
        internal InheritableParamAttr(CXCursor handle) : base(handle)
        {
            if (handle.AttrKind is > CX_AttrKind.CX_AttrKind_LastInheritableParamAttr or < CX_AttrKind.CX_AttrKind_FirstInheritableParamAttr)
            {
                throw new ArgumentOutOfRangeException(nameof(handle));
            }
        }
    }
}
