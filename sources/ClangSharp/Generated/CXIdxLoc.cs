namespace ClangSharp
{
    public partial struct CXIdxLoc
    {
        [NativeTypeName("void *[2]")]
        public _ptr_data_e__FixedBuffer ptr_data;

        [NativeTypeName("unsigned int")]
        public uint int_data;

        public unsafe partial struct _ptr_data_e__FixedBuffer
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
