namespace ClangSharp
{
    using System;
    using System.Runtime.InteropServices;

    public partial struct CXIdxObjCPropertyDeclInfo
    {
        public IntPtr declInfo;
        public IntPtr getter;
        public IntPtr setter;
    }
}
