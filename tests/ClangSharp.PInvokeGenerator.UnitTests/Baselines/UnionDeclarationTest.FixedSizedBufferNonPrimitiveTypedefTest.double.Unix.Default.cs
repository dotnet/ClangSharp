using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct MyUnion
    {
        [FieldOffset(0)]
        public double value;
    }

    [StructLayout(LayoutKind.Explicit)]
    public partial struct MyOtherUnion
    {
        [FieldOffset(0)]
        [NativeTypeName("MyBuffer")]
        public _c_e__FixedBuffer c;

        public partial struct _c_e__FixedBuffer
        {
            public MyUnion e0;
            public MyUnion e1;
            public MyUnion e2;

            public ref MyUnion this[int index]
            {
                get
                {
                    return ref AsSpan()[index];
                }
            }

            public Span<MyUnion> AsSpan() => MemoryMarshal.CreateSpan(ref e0, 3);
        }
    }
}
