// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ParameterABIAttr : InheritableParamAttr
    {
        internal ParameterABIAttr(CXCursor handle) : base(handle)
        {
            if ((CX_AttrKind.CX_AttrKind_LastParameterABIAttr < handle.AttrKind) || (handle.AttrKind < CX_AttrKind.CX_AttrKind_FirstParameterABIAttr))
            {
                throw new ArgumentException(nameof(handle));
            }
        }
    }
}
