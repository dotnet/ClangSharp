namespace ClangSharp.Test
{
    public unsafe partial struct WithDestructor
    {
        public int* data;

        public void Dispose()
        {
            if (data != null)
            {
                cxx_delete(data);
            }
        }
    }
}
