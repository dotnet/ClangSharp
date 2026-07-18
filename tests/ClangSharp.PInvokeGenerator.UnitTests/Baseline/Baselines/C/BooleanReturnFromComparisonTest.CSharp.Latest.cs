namespace ClangSharp.Test
{
    public static partial class Methods
    {
        [return: NativeTypeName("_Bool")]
        public static bool FromComparison(int a, int b)
        {
            return a < b;
        }

        [return: NativeTypeName("_Bool")]
        public static bool FromLogical(int a, int b)
        {
            return a < b || a > b;
        }

        [return: NativeTypeName("_Bool")]
        public static bool FromInteger(int a)
        {
            return (a) != 0;
        }
    }
}
