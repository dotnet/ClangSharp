// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Runtime.InteropServices;

namespace ClangSharp.Interop
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: NativeTypeName("enum CXChildVisitResult")]
    public unsafe delegate CXChildVisitResult CXCursorVisitor(CXCursor cursor, CXCursor parent, [NativeTypeName("CXClientData")] void* client_data);
}
