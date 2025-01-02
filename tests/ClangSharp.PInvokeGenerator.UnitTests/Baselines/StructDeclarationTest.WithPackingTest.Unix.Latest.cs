using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [StructLayout(LayoutKind.Sequential, Pack = CustomPackValue)]
    public partial struct MyStruct
    {
        [NativeTypeName("size_t[2]")]
        public _FixedBuffer_e__FixedBuffer FixedBuffer;

        [InlineArray(2)]
        public partial struct _FixedBuffer_e__FixedBuffer
        {
            public nuint e0;
        }
    }
}
