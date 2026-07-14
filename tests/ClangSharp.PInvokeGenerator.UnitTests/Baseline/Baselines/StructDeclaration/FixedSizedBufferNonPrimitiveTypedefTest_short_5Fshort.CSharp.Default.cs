using System.Runtime.CompilerServices;

namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        public short value;
    }

    public partial struct MyOtherStruct
    {
        [NativeTypeName("MyBuffer")]
        public _c_e__FixedBuffer c;

        [InlineArray(3)]
        public partial struct _c_e__FixedBuffer
        {
            public MyStruct e0;
        }
    }
}
