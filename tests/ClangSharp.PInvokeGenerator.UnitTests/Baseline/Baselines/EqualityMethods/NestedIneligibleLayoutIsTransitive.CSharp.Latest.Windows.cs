using System.Runtime.CompilerServices;

namespace ClangSharp.Test
{
    public partial struct Inner
    {
        [NativeTypeName("int[4]")]
        public _Values_e__FixedBuffer Values;

        [InlineArray(4)]
        public partial struct _Values_e__FixedBuffer
        {
            public int e0;
        }
    }

    public partial struct Outer
    {
        public int Id;

        public Inner Data;
    }
}
