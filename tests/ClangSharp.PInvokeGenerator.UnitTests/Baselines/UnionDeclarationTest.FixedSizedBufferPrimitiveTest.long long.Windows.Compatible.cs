using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [StructLayout(LayoutKind.Explicit)]
    public unsafe partial struct MyUnion
    {
        [FieldOffset(0)]
        [NativeTypeName("long long[3]")]
        public fixed long c[3];
    }
}
