using System;

namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        public int MyFunction0()
        {
            return 0;
        }

        [Obsolete]
        public int MyFunction1()
        {
            return 0;
        }

        [Obsolete("This is obsolete.")]
        public int MyFunction2()
        {
            return 0;
        }

        public int MyFunction3()
        {
            return 0;
        }
    }
}
