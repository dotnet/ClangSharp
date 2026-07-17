// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;

namespace ClangSharp;

public sealed class InclusionDirective : PreprocessingDirective
{
    internal InclusionDirective(CXCursor handle) : base(handle, CXCursor_InclusionDirective)
    {
    }

    public CX_InclusionDirectiveKind Kind => Handle.InclusionDirectiveKind;

    public bool WasInQuotes => Handle.InclusionDirectiveWasInQuotes;
}
