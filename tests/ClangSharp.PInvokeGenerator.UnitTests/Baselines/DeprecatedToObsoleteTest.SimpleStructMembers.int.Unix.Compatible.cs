using System;

namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        public int r;

        [Obsolete]
        public int g;

        [Obsolete("This is obsolete.")]
        public int b;

        public int a;
    }
}
