using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct MyUnion
    {
        [FieldOffset(0)]
        [NativeTypeName("MyTypedefAlias")]
        public float r;

        [FieldOffset(0)]
        [NativeTypeName("MyTypedefAlias")]
        public float g;

        [FieldOffset(0)]
        [NativeTypeName("MyTypedefAlias")]
        public float b;
    }
}
