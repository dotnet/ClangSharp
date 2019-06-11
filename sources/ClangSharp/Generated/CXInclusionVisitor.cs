using System.Runtime.InteropServices;

namespace ClangSharp
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void CXInclusionVisitor([NativeTypeName("CXFile")] void* included_file, [NativeTypeName("CXSourceLocation *")] CXSourceLocation* inclusion_stack, [NativeTypeName("unsigned int")] uint include_len, [NativeTypeName("CXClientData")] void* client_data);
}
