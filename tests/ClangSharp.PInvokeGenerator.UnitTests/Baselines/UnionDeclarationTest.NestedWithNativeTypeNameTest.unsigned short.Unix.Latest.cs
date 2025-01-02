using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct MyUnion
    {
        [FieldOffset(0)]
        [NativeTypeName("unsigned short")]
        public ushort r;

        [FieldOffset(0)]
        [NativeTypeName("unsigned short")]
        public ushort g;

        [FieldOffset(0)]
        [NativeTypeName("unsigned short")]
        public ushort b;

        [StructLayout(LayoutKind.Explicit)]
        public partial struct MyNestedUnion
        {
            [FieldOffset(0)]
            [NativeTypeName("unsigned short")]
            public ushort r;

            [FieldOffset(0)]
            [NativeTypeName("unsigned short")]
            public ushort g;

            [FieldOffset(0)]
            [NativeTypeName("unsigned short")]
            public ushort b;

            [FieldOffset(0)]
            [NativeTypeName("unsigned short")]
            public ushort a;
        }
    }
}
