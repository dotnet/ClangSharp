namespace ClangSharp
{
    using System;
    using System.Runtime.InteropServices;

    public partial struct CXStringSet
    {
        public IntPtr Strings;
        public uint Count;
    }
}
