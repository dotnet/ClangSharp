using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct MyUnion
    {
        [FieldOffset(0)]
        [NativeTypeName("MyBuffer")]
        public _c_e__FixedBuffer c;

        [InlineArray(3)]
        public partial struct _c_e__FixedBuffer
        {
            public short e0;
        }
    }
}
