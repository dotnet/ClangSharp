using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct MyUnion
    {
        [FieldOffset(0)]
        [NativeTypeName("MyTypedefAlias")]
        public long r;

        [FieldOffset(0)]
        [NativeTypeName("MyTypedefAlias")]
        public long g;

        [FieldOffset(0)]
        [NativeTypeName("MyTypedefAlias")]
        public long b;
    }
}
