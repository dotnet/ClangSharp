// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;

namespace ClangSharp.Interop
{
    public unsafe partial struct CXCursorAndRangeVisitor
    {
        [NativeTypeName("void *")]
        public void* context;

        [NativeTypeName("enum CXVisitorResult (*)(void *, CXCursor, CXSourceRange)")]
        public IntPtr visit;
    }
}
