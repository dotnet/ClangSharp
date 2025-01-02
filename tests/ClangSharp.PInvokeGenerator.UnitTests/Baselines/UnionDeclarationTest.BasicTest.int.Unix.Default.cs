using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct MyUnion
    {
        [FieldOffset(0)]
        public int r;

        [FieldOffset(0)]
        public int g;

        [FieldOffset(0)]
        public int b;
    }
}
