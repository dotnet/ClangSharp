namespace ClangSharp.Test
{
    public unsafe partial struct context_t
    {
        [NativeTypeName("unsigned char *")]
        public byte* buf;

        public int size;
    }

    public static unsafe partial class Methods
    {
        public static int buf_close(void* pContext)
        {
            ((context_t*)(pContext))->buf = null;
            return 0;
        }
    }
}
