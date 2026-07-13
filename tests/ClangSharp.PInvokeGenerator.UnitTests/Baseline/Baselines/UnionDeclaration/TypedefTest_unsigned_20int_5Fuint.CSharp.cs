using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct MyUnion
    {
        [FieldOffset(0)]
        [NativeTypeName("MyTypedefAlias")]
        public uint r;

        [FieldOffset(0)]
        [NativeTypeName("MyTypedefAlias")]
        public uint g;

        [FieldOffset(0)]
        [NativeTypeName("MyTypedefAlias")]
        public uint b;
    }
}
