namespace ClangSharp.Test
{
    public static unsafe partial class Methods
    {
        public static int* MyFunction(void* input)
        {
            return (int*)(input);
        }
    }
}
