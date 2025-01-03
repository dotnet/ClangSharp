namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
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
