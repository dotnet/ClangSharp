using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [StructLayout(LayoutKind.Explicit)]
    public unsafe partial struct example_s
    {
        [FieldOffset(0)]
        public example_s* next;

        [FieldOffset(0)]
        public void* data;
    }
}
