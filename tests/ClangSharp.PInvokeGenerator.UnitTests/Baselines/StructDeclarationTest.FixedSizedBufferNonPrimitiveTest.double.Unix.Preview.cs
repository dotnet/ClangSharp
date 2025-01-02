using System.Runtime.CompilerServices;

namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        public double value;
    }

    public partial struct MyOtherStruct
    {
        [NativeTypeName("MyStruct[3]")]
        public _c_e__FixedBuffer c;

        [InlineArray(3)]
        public partial struct _c_e__FixedBuffer
        {
            public MyStruct e0;
        }
    }
}
