// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using ClangSharp.Interop;

namespace ClangSharp
{
    public class PreprocessingDirective : PreprocessedEntity
    {
        internal PreprocessingDirective(CXCursor handle) : this(handle, CXCursorKind.CXCursor_PreprocessingDirective)
        {
        }

        private protected PreprocessingDirective(CXCursor handle, CXCursorKind expectedCursorKind) : base(handle, expectedCursorKind)
        {
        }
    }
}
