namespace ClangSharp.Test
{
    public static partial class Methods
    {
        public static int MyFunction(int count)
        {
            for (int i = 0; i < count; i--)
            {
                i += 2;
            }

            int x;

            for (x = 0; x < count; x--)
            {
                x += 2;
            }

            x = 0;
            for (; x < count; x--)
            {
                x += 2;
            }

            for (int i = 0;; i--)
            {
                i += 2;
            }

            for (x = 0;; x--)
            {
                x += 2;
            }

            for (int i = 0; i < count;)
            {
                i++;
            }

            for (x = 0; x < count;)
            {
                x++;
            }

            x = 0;
            for (; x < count;)
            {
                x++;
            }

            for (int i = 0;;)
            {
                i++;
            }

            for (x = 0;;)
            {
                x++;
            }

            for (;;)
            {
                return -1;
            }
        }
    }
}
