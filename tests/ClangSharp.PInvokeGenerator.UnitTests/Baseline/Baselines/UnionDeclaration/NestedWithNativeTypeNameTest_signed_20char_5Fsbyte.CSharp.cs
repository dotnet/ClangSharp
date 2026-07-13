using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct MyUnion
    {
        [FieldOffset(0)]
        [NativeTypeName("signed char")]
        public sbyte r;

        [FieldOffset(0)]
        [NativeTypeName("signed char")]
        public sbyte g;

        [FieldOffset(0)]
        [NativeTypeName("signed char")]
        public sbyte b;

        [StructLayout(LayoutKind.Explicit)]
        public partial struct MyNestedUnion
        {
            [FieldOffset(0)]
            [NativeTypeName("signed char")]
            public sbyte r;

            [FieldOffset(0)]
            [NativeTypeName("signed char")]
            public sbyte g;

            [FieldOffset(0)]
            [NativeTypeName("signed char")]
            public sbyte b;

            [FieldOffset(0)]
            [NativeTypeName("signed char")]
            public sbyte a;
        }
    }
}
