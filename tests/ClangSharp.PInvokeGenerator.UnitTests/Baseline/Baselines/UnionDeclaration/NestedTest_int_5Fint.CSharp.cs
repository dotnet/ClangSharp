using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct MyUnion
    {
        [FieldOffset(0)]
        public int r;

        [FieldOffset(0)]
        public int g;

        [FieldOffset(0)]
        public int b;

        [StructLayout(LayoutKind.Explicit)]
        public partial struct MyNestedUnion
        {
            [FieldOffset(0)]
            public int r;

            [FieldOffset(0)]
            public int g;

            [FieldOffset(0)]
            public int b;

            [FieldOffset(0)]
            public int a;
        }
    }
}
