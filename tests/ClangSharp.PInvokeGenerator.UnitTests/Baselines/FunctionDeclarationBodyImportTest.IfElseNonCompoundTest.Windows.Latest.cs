namespace ClangSharp.Test
{
    public static partial class Methods
    {
        public static int MyFunction(bool condition, int lhs, int rhs)
        {
            if (condition)
            {
                return lhs;
            }
            else
            {
                return rhs;
            }
        }
    }
}
