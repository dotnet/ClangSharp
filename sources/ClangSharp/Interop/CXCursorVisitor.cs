using System.Runtime.InteropServices;

namespace ClangSharp.Interop
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: NativeTypeName("enum CXChildVisitResult")]
    public unsafe delegate CXChildVisitResult CXCursorVisitor(CXCursor cursor, CXCursor parent, [NativeTypeName("CXClientData")] void* client_data);
}
