using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [StructLayout(LayoutKind.Explicit)]
    public unsafe partial struct MyUnion
    {
        [FieldOffset(0)]
        [NativeTypeName("signed char[2][1][3][4]")]
        public fixed sbyte c[2 * 1 * 3 * 4];
    }
}
