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
        [NativeTypeName("MyBuffer")]
        public _c_e__FixedBuffer c;

        public partial struct _c_e__FixedBuffer
        {
            public MyUnion e0;
            public MyUnion e1;
            public MyUnion e2;

            public unsafe ref MyUnion this[int index]
            {
                get
                {
                    fixed (MyUnion* pThis = &e0)
                    {
                        return ref pThis[index];
                    }
                }
            }
        }
    }
}
