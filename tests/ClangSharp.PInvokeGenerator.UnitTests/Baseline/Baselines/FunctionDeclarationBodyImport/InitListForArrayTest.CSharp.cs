namespace ClangSharp.Test
{
    public static partial class Methods
    {
        public static void MyFunction()
        {
            int[] x = new int[4]
            {
                1,
                2,
                3,
                4,
            };
            int[] y = new int[4]
            {
                1,
                2,
                3,
                default,
            };
            int[] z = new int[2]
            {
                1,
                2,
            };
        }
    }
}
