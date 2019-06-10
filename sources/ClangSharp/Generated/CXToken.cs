namespace ClangSharp
{
    public unsafe partial struct CXToken
    {
        public fixed uint int_data[4];

        public void* ptr_data;
    }
}
