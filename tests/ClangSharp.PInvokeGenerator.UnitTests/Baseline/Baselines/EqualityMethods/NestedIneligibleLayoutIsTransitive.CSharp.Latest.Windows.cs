namespace ClangSharp.Test
{
    public partial struct Inner
    {
        [NativeTypeName("void *[4]")]
        public _Handles_e__FixedBuffer Handles;

        public unsafe partial struct _Handles_e__FixedBuffer
        {
            public void* e0;
            public void* e1;
            public void* e2;
            public void* e3;

            public ref void* this[int index]
            {
                get
                {
                    fixed (void** pThis = &e0)
                    {
                        return ref pThis[index];
                    }
                }
            }
        }
    }

    public partial struct Outer
    {
        public int Id;

        public Inner Data;
    }
}
