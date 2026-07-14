using System;

namespace ClangSharp.Test
{
    public static partial class Methods
    {
        public static int MyVariable0 = 0;

        [Obsolete]
        public static int MyVariable1 = 0;

        [Obsolete("This is obsolete.")]
        public static int MyVariable2 = 0;

        public static int MyVariable3 = 0;
    }
}
