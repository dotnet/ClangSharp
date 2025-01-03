using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        public int value;
    }

    public partial struct MyOtherStruct
    {
        [NativeTypeName("MyStruct[3]")]
        public _c_e__FixedBuffer c;

        public partial struct _c_e__FixedBuffer
        {
            public MyStruct e0;
            public MyStruct e1;
            public MyStruct e2;

            public ref MyStruct this[int index]
            {
                get
                {
                    return ref AsSpan()[index];
                }
            }

            public Span<MyStruct> AsSpan() => MemoryMarshal.CreateSpan(ref e0, 3);
        }
    }
}
