using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [StructLayout(LayoutKind.Explicit)]
    public unsafe partial struct MyUnion
    {
        [FieldOffset(0)]
        [NativeTypeName("long long[2][1][3][4]")]
        public fixed long c[2 * 1 * 3 * 4];
    }
}
