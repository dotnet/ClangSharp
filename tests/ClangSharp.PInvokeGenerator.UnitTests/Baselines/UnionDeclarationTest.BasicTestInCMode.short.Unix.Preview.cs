using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct MyUnion
    {
        [FieldOffset(0)]
        public short r;

        [FieldOffset(0)]
        public short g;

        [FieldOffset(0)]
        public short b;
    }
}
