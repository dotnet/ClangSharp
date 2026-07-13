using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct MyUnion
    {
        [FieldOffset(0)]
        public double r;

        [FieldOffset(0)]
        public double g;

        [FieldOffset(0)]
        public double b;

        [StructLayout(LayoutKind.Explicit)]
        public partial struct MyNestedUnion
        {
            [FieldOffset(0)]
            public double r;

            [FieldOffset(0)]
            public double g;

            [FieldOffset(0)]
            public double b;

            [FieldOffset(0)]
            public double a;
        }
    }
}
