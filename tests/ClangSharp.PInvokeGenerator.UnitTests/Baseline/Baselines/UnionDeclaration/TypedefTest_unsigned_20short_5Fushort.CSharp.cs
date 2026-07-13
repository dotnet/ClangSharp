using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct MyUnion
    {
        [FieldOffset(0)]
        [NativeTypeName("MyTypedefAlias")]
        public ushort r;

        [FieldOffset(0)]
        [NativeTypeName("MyTypedefAlias")]
        public ushort g;

        [FieldOffset(0)]
        [NativeTypeName("MyTypedefAlias")]
        public ushort b;
    }
}
