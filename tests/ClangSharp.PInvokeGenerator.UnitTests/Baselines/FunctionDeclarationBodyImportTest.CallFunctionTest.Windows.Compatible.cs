namespace ClangSharp.Test
{
    public static partial class Methods
    {
        public static void MyCalledFunction()
        {
        }

        public static void MyFunction()
        {
            MyCalledFunction();
        }
    }
}
