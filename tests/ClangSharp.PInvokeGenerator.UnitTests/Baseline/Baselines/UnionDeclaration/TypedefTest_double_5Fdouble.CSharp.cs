using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct MyUnion
    {
        [FieldOffset(0)]
        [NativeTypeName("MyTypedefAlias")]
        public double r;

        [FieldOffset(0)]
        [NativeTypeName("MyTypedefAlias")]
        public double g;

        [FieldOffset(0)]
        [NativeTypeName("MyTypedefAlias")]
        public double b;
    }
}
