using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public unsafe partial struct MyStruct1
    {
        [NativeTypeName("unsigned int")]
        public uint Field1;

        public void* Field2;

        [NativeTypeName("unsigned int")]
        public uint Field3;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public unsafe partial struct MyStruct2
    {
        [NativeTypeName("unsigned int")]
        public uint Field1;

        public void* Field2;

        [NativeTypeName("unsigned int")]
        public uint Field3;
    }
}
