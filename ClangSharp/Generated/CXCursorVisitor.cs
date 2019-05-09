namespace ClangSharp
{
    using System;
    using System.Runtime.InteropServices;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate CXChildVisitResult CXCursorVisitor(CXCursor cursor, CXCursor parent, CXClientData client_data);
}
