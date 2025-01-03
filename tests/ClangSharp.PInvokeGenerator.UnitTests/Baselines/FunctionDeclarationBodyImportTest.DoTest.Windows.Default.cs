namespace ClangSharp.Test
{
    public static partial class Methods
    {
        public static int MyFunction(int count)
        {
            int i = 0;

            do
            {
                i++;
            }
            while (i < count);

            return i;
        }
    }
}
