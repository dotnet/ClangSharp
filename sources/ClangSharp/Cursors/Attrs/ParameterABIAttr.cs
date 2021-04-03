// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ParameterABIAttr : InheritableParamAttr
    {
        internal ParameterABIAttr(CXCursor handle) : base(handle)
        {
            if (handle.AttrKind is > CX_AttrKind.CX_AttrKind_LastParameterABIAttr or < CX_AttrKind.CX_AttrKind_FirstParameterABIAttr)
            {
                throw new ArgumentOutOfRangeException(nameof(handle));
            }
        }
    }
}
