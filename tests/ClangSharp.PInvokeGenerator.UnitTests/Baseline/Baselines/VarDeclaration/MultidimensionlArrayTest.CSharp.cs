namespace ClangSharp.Test
{
    public static partial class Methods
    {
        [NativeTypeName("const int[2][2]")]
        public static readonly int[][] MyArray = new int[2][]
        {
            new int[2]
            {
                0,
                1,
            },
            new int[2]
            {
                2,
                3,
            },
        };
    }
}
