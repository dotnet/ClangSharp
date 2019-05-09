namespace ClangSharp
{
    using System;
    using System.Runtime.InteropServices;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void CXInclusionVisitor(CXFile included_file, out CXSourceLocation inclusion_stack, uint include_len, CXClientData client_data);
}
