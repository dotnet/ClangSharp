namespace ClangSharp.Test
{
    public partial struct Point
    {
        public int x;

        public int y;
    }

    public static partial class Methods
    {
        public static Point MyGlobalPoint = new Point
        {
            x = 10,
            y = 20,
        };
    }
}
