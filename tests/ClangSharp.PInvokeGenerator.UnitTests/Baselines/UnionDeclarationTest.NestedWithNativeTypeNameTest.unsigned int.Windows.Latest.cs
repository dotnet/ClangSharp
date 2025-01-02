using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct MyUnion
    {
        [FieldOffset(0)]
        [NativeTypeName("unsigned int")]
        public uint r;

        [FieldOffset(0)]
        [NativeTypeName("unsigned int")]
        public uint g;

        [FieldOffset(0)]
        [NativeTypeName("unsigned int")]
        public uint b;

        [StructLayout(LayoutKind.Explicit)]
        public partial struct MyNestedUnion
        {
            [FieldOffset(0)]
            [NativeTypeName("unsigned int")]
            public uint r;

            [FieldOffset(0)]
            [NativeTypeName("unsigned int")]
            public uint g;

            [FieldOffset(0)]
            [NativeTypeName("unsigned int")]
            public uint b;

            [FieldOffset(0)]
            [NativeTypeName("unsigned int")]
            public uint a;
        }
    }
}
