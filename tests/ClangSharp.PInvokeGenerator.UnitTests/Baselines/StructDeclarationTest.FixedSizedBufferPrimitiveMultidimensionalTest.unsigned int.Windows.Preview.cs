using System.Runtime.CompilerServices;

namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        [NativeTypeName("unsigned int[2][1][3][4]")]
        public _c_e__FixedBuffer c;

        [InlineArray(2 * 1 * 3 * 4)]
        public partial struct _c_e__FixedBuffer
        {
            public uint e0_0_0_0;
        }
    }
}
