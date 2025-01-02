using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct MyUnion
    {
        [FieldOffset(0)]
        [NativeTypeName("double *[3]")]
        public _c_e__FixedBuffer c;

        public unsafe partial struct _c_e__FixedBuffer
        {
            public double* e0;
            public double* e1;
            public double* e2;

            public ref double* this[int index]
            {
                get
                {
                    fixed (double** pThis = &e0)
                    {
                        return ref pThis[index];
                    }
                }
            }
        }
    }
}
