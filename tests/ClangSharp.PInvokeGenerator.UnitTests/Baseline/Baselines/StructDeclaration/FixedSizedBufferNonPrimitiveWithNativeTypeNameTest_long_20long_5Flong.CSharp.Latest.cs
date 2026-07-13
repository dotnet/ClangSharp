using System.Runtime.CompilerServices;

namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        [NativeTypeName("long long")]
        public long value;
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
