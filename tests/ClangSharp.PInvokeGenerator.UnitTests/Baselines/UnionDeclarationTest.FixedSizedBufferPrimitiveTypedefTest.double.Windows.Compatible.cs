using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [StructLayout(LayoutKind.Explicit)]
    public unsafe partial struct MyUnion
    {
        [FieldOffset(0)]
        [NativeTypeName("MyBuffer")]
        public fixed double c[3];
    }
}
