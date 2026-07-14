using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [StructLayout(LayoutKind.Explicit)]
    public unsafe partial struct MyUnion
    {
        [FieldOffset(0)]
        [NativeTypeName("signed char[3]")]
        public fixed sbyte c[3];
    }
}
