namespace ClangSharp.Test
{
    public static partial class Methods
    {
        public static void MyCalledFunction(int x, int y)
        {
        }

        public static void MyFunction()
        {
            MyCalledFunction(0, 1);
        }
    }
}
