using System.Runtime.InteropServices;

namespace ClangSharp
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: NativeTypeName("enum CXVisitorResult")]
    public unsafe delegate CXVisitorResult CXFieldVisitor(CXCursor C, [NativeTypeName("CXClientData")] void* client_data);
}
