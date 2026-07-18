namespace ClangSharp.Test
{
    public static partial class Methods
    {
        public static int LogicalAnd([NativeTypeName("_Bool")] bool a, [NativeTypeName("_Bool")] bool b)
        {
            return (a && b) ? 1 : 0;
        }

        public static int LogicalOr([NativeTypeName("_Bool")] bool a, [NativeTypeName("_Bool")] bool b)
        {
            return (a || b) ? 1 : 0;
        }

        public static int MixedOperands([NativeTypeName("_Bool")] bool a, int b)
        {
            return (a && (b < 3)) ? 1 : 0;
        }

        public static void LogicalLocal([NativeTypeName("_Bool")] bool a, [NativeTypeName("_Bool")] bool b)
        {
            int x = (a || b) ? 1 : 0;
        }
    }
}
