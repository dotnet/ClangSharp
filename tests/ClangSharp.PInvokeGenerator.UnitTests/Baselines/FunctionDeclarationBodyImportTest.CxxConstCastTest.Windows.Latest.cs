namespace ClangSharp.Test
{
    public static unsafe partial class Methods
    {
        public static void* MyFunction([NativeTypeName("const void *")] void* input)
        {
            return input;
        }
    }
}
