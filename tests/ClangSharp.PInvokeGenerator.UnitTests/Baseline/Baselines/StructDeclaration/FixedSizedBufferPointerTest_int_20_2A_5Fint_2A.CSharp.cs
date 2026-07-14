namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        [NativeTypeName("int *[3]")]
        public _c_e__FixedBuffer c;

        public unsafe partial struct _c_e__FixedBuffer
        {
            public int* e0;
            public int* e1;
            public int* e2;

            public ref int* this[int index]
            {
                get
                {
                    fixed (int** pThis = &e0)
                    {
                        return ref pThis[index];
                    }
                }
            }
        }
    }
}
