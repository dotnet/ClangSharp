namespace ClangSharp.Test
{
    public static partial class Methods
    {
        public static int ReturnFromComparison(int a, int b)
        {
            return (a < b) ? 1 : 0;
        }

        public static int ReturnFromLogical(int a, int b)
        {
            return (a < b || a > b) ? 1 : 0;
        }

        public static int ReturnFromNegation(int a)
        {
            return (a == 0) ? 1 : 0;
        }

        public static void Locals(int a, int b)
        {
            int x = (a < b) ? 1 : 0;
            int y = (a == 0) ? 1 : 0;
        }
    }
}
