namespace ClangSharp
{
    using System;
    using System.Runtime.InteropServices;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate CXVisitorResult CXFieldVisitor(CXCursor C, CXClientData client_data);
}
