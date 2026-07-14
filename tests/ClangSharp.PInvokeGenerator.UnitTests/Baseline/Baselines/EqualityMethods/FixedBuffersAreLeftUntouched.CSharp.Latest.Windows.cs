using System.Runtime.CompilerServices;

namespace ClangSharp.Test
{
    public partial struct WithArray
    {
        [NativeTypeName("int[4]")]
        public _Values_e__FixedBuffer Values;

        [InlineArray(4)]
        public partial struct _Values_e__FixedBuffer
        {
            public int e0;
        }
    }
}
