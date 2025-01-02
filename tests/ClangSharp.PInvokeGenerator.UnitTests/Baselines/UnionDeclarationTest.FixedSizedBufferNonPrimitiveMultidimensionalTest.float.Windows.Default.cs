using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct MyUnion
    {
        [FieldOffset(0)]
        public float value;
    }

    [StructLayout(LayoutKind.Explicit)]
    public partial struct MyOtherUnion
    {
        [FieldOffset(0)]
        [NativeTypeName("MyUnion[2][1][3][4]")]
        public _c_e__FixedBuffer c;

        public partial struct _c_e__FixedBuffer
        {
            public MyUnion e0_0_0_0;
            public MyUnion e1_0_0_0;

            public MyUnion e0_0_1_0;
            public MyUnion e1_0_1_0;

            public MyUnion e0_0_2_0;
            public MyUnion e1_0_2_0;

            public MyUnion e0_0_0_1;
            public MyUnion e1_0_0_1;

            public MyUnion e0_0_1_1;
            public MyUnion e1_0_1_1;

            public MyUnion e0_0_2_1;
            public MyUnion e1_0_2_1;

            public MyUnion e0_0_0_2;
            public MyUnion e1_0_0_2;

            public MyUnion e0_0_1_2;
            public MyUnion e1_0_1_2;

            public MyUnion e0_0_2_2;
            public MyUnion e1_0_2_2;

            public MyUnion e0_0_0_3;
            public MyUnion e1_0_0_3;

            public MyUnion e0_0_1_3;
            public MyUnion e1_0_1_3;

            public MyUnion e0_0_2_3;
            public MyUnion e1_0_2_3;

            public ref MyUnion this[int index]
            {
                get
                {
                    return ref AsSpan()[index];
                }
            }

            public Span<MyUnion> AsSpan() => MemoryMarshal.CreateSpan(ref e0_0_0_0, 24);
        }
    }
}
