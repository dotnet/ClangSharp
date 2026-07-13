namespace ClangSharp.Test
{
    public static partial class Methods
    {
        public static int MyFunction(int value)
        {
            switch (value)
            {
                case 0:
                {
                    return 0;
                }

                case 2:
                case 3:
                {
                    return 5;
                }
            }

            return -1;
        }
    }
}
