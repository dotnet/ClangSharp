using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct MyUnion
    {
        [FieldOffset(0)]
        [NativeTypeName("long long")]
        public long r;

        [FieldOffset(0)]
        [NativeTypeName("long long")]
        public long g;

        [FieldOffset(0)]
        [NativeTypeName("long long")]
        public long b;

        [StructLayout(LayoutKind.Explicit)]
        public partial struct MyNestedUnion
        {
            [FieldOffset(0)]
            [NativeTypeName("long long")]
            public long r;

            [FieldOffset(0)]
            [NativeTypeName("long long")]
            public long g;

            [FieldOffset(0)]
            [NativeTypeName("long long")]
            public long b;

            [FieldOffset(0)]
            [NativeTypeName("long long")]
            public long a;
        }
    }
}
