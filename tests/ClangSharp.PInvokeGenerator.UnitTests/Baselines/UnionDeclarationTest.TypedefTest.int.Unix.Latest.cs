using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct MyUnion
    {
        [FieldOffset(0)]
        [NativeTypeName("MyTypedefAlias")]
        public int r;

        [FieldOffset(0)]
        [NativeTypeName("MyTypedefAlias")]
        public int g;

        [FieldOffset(0)]
        [NativeTypeName("MyTypedefAlias")]
        public int b;
    }
}
