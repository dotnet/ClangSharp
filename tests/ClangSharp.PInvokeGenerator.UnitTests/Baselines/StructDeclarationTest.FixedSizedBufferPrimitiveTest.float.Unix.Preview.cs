using System.Runtime.CompilerServices;

namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        [NativeTypeName("float[3]")]
        public _c_e__FixedBuffer c;

        [InlineArray(3)]
        public partial struct _c_e__FixedBuffer
        {
            public float e0;
        }
    }
}
