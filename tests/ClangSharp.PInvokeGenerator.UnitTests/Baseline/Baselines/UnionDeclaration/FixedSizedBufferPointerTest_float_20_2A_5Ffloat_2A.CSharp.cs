using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct MyUnion
    {
        [FieldOffset(0)]
        [NativeTypeName("float *[3]")]
        public _c_e__FixedBuffer c;

        public unsafe partial struct _c_e__FixedBuffer
        {
            public float* e0;
            public float* e1;
            public float* e2;

            public ref float* this[int index]
            {
                get
                {
                    fixed (float** pThis = &e0)
                    {
                        return ref pThis[index];
                    }
                }
            }
        }
    }
}
