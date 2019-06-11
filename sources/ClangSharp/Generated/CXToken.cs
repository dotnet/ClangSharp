namespace ClangSharp
{
    public unsafe partial struct CXToken
    {
        [NativeTypeName("unsigned int [4]")]
        public fixed uint int_data[4];

        [NativeTypeName("void *")]
        public void* ptr_data;
    }
}
