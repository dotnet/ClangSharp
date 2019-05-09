using System.Runtime.InteropServices;

namespace ClangSharp
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate CXVisitorResult CXFieldVisitor(CXCursor C, CXClientData client_data);
}
