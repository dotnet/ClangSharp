// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_AttrKind;

namespace ClangSharp;

public class HLSLAnnotationAttr : InheritableAttr
{
    internal HLSLAnnotationAttr(CXCursor handle) : base(handle)
    {
        if (handle.AttrKind is > CX_AttrKind_LastHLSLAnnotationAttr or < CX_AttrKind_FirstHLSLAnnotationAttr)
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
        }
    }
}
