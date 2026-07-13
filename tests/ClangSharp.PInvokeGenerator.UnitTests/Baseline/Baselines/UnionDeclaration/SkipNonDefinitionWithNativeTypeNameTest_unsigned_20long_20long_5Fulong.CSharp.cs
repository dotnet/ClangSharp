using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct MyUnion
    {
        [FieldOffset(0)]
        [NativeTypeName("unsigned long long")]
        public ulong r;

        [FieldOffset(0)]
        [NativeTypeName("unsigned long long")]
        public ulong g;

        [FieldOffset(0)]
        [NativeTypeName("unsigned long long")]
        public ulong b;
    }
}
