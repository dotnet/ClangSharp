using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct MyUnion
    {
        [FieldOffset(0)]
        [NativeTypeName("MyTypedefAlias")]
        public short r;

        [FieldOffset(0)]
        [NativeTypeName("MyTypedefAlias")]
        public short g;

        [FieldOffset(0)]
        [NativeTypeName("MyTypedefAlias")]
        public short b;
    }
}
