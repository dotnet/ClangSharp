// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;

namespace ClangSharp;

public class PreprocessingDirective : PreprocessedEntity
{
    internal PreprocessingDirective(CXCursor handle) : this(handle, CXCursorKind.CXCursor_PreprocessingDirective)
    {
    }

    private protected PreprocessingDirective(CXCursor handle, CXCursorKind expectedCursorKind) : base(handle, expectedCursorKind)
    {
    }
}
