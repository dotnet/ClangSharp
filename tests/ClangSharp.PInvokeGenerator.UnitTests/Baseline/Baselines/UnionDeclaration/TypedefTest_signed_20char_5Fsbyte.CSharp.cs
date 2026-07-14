using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct MyUnion
    {
        [FieldOffset(0)]
        [NativeTypeName("MyTypedefAlias")]
        public sbyte r;

        [FieldOffset(0)]
        [NativeTypeName("MyTypedefAlias")]
        public sbyte g;

        [FieldOffset(0)]
        [NativeTypeName("MyTypedefAlias")]
        public sbyte b;
    }
}
