namespace ClangSharp.Test
{
    public static unsafe partial class Methods
    {
        public static void* MyFunction(void* buf, [NativeTypeName("unsigned long long")] ulong size)
        {
            return (byte*)buf + size;
        }
    }
}
