namespace ClangSharp.Test
{
    public static partial class Methods
    {
        public static int MyFunction(bool condition, int lhs, int rhs)
        {
            return condition ? lhs : rhs;
        }
    }
}
