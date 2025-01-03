namespace ClangSharp.Test
{
    public static unsafe partial class Methods
    {
        public static int* MyFunction(int value)
        {
            return &value;
        }
    }
}
