using System;

namespace ClangSharp.Test
{
    public partial struct MyStruct0
    {
        public int r;
    }

    [Obsolete]
    public partial struct MyStruct1
    {
        public int r;
    }

    [Obsolete("This is obsolete.")]
    public partial struct MyStruct2
    {
        public int r;
    }

    public partial struct MyStruct3
    {
        public int r;
    }
}
