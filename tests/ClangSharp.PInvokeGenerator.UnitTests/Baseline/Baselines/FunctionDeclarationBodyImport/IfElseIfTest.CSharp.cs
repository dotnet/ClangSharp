namespace ClangSharp.Test
{
    public static partial class Methods
    {
        public static int MyFunction(bool condition1, int a, int b, bool condition2, int c)
        {
            if (condition1)
            {
                return a;
            }
            else if (condition2)
            {
                return b;
            }

            return c;
        }
    }
}
