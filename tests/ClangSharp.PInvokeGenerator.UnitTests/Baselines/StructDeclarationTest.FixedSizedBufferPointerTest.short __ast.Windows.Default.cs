namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        [NativeTypeName("short *[3]")]
        public _c_e__FixedBuffer c;

        public unsafe partial struct _c_e__FixedBuffer
        {
            public short* e0;
            public short* e1;
            public short* e2;

            public ref short* this[int index]
            {
                get
                {
                    fixed (short** pThis = &e0)
                    {
                        return ref pThis[index];
                    }
                }
            }
        }
    }
}
