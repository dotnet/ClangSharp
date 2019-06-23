#if Windows_NT
using nulong = System.UInt32;
#else
using System;
using nulong = System.UIntPtr;
#endif

namespace ClangSharp.Interop
{
    public unsafe partial struct CXUnsavedFile
    {
        [NativeTypeName("const char *")]
        public sbyte* Filename;

        [NativeTypeName("const char *")]
        public sbyte* Contents;

        [NativeTypeName("unsigned long")]
        public nulong Length;
    }
}
