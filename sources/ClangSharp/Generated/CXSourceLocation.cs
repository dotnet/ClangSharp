namespace ClangSharp
{
    public partial struct CXSourceLocation
    {
        [NativeTypeName("const void*[2]")]
        public _ptr_data_e__FixedBuffer ptr_data;

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
