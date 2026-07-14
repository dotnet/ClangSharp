using System;

namespace ClangSharp.Test
{
    public static partial class Methods
    {
        public static void MyFunction0()
        {
        }

        [Obsolete]
        public static void MyFunction1()
        {
        }

        [Obsolete("This is obsolete.")]
        public static void MyFunction2()
        {
        }

        public static void MyFunction3()
        {
        }
    }
}
