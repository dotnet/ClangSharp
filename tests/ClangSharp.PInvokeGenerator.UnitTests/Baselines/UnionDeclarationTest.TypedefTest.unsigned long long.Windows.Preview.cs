using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct MyUnion
    {
        [FieldOffset(0)]
        [NativeTypeName("MyTypedefAlias")]
        public ulong r;

        [FieldOffset(0)]
        [NativeTypeName("MyTypedefAlias")]
        public ulong g;

        [FieldOffset(0)]
        [NativeTypeName("MyTypedefAlias")]
        public ulong b;
    }
}
