namespace ClangSharp.Interop
{
    public partial struct CXCursor
    {
        [NativeTypeName("enum CXCursorKind")]
        public CXCursorKind kind;

        public int xdata;

        [NativeTypeName("const void *[3]")]
        public _data_e__FixedBuffer data;

        public unsafe partial struct _data_e__FixedBuffer
        {
            internal void* e0;
            internal void* e1;
            internal void* e2;

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
}
