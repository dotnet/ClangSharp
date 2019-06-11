namespace ClangSharp
{
    public partial struct CXType
    {
        [NativeTypeName("enum CXTypeKind")]
        public CXTypeKind kind;

        [NativeTypeName("void *[2]")]
        public _data_e__FixedBuffer data;

        public unsafe partial struct _data_e__FixedBuffer
        {
            internal void* e0;
            internal void* e1;

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
