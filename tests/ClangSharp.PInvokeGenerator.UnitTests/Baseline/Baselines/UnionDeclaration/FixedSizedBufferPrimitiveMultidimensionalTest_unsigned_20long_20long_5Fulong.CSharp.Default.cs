using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct MyUnion
    {
        [FieldOffset(0)]
        [NativeTypeName("unsigned long long[2][1][3][4]")]
        public _c_e__FixedBuffer c;

        [InlineArray(2 * 1 * 3 * 4)]
        public partial struct _c_e__FixedBuffer
        {
            public ulong e0_0_0_0;
        }
    }
}
