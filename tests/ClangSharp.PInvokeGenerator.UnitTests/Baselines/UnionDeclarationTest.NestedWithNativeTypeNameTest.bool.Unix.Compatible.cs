using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct MyUnion
    {
        [FieldOffset(0)]
        [NativeTypeName("bool")]
        public byte r;

        [FieldOffset(0)]
        [NativeTypeName("bool")]
        public byte g;

        [FieldOffset(0)]
        [NativeTypeName("bool")]
        public byte b;

        [StructLayout(LayoutKind.Explicit)]
        public partial struct MyNestedUnion
        {
            [FieldOffset(0)]
            [NativeTypeName("bool")]
            public byte r;

            [FieldOffset(0)]
            [NativeTypeName("bool")]
            public byte g;

            [FieldOffset(0)]
            [NativeTypeName("bool")]
            public byte b;

            [FieldOffset(0)]
            [NativeTypeName("bool")]
            public byte a;
        }
    }
}
