using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [StructLayout(LayoutKind.Sequential, Pack = 16)]
    [NativeAlignment(16)]
    public partial struct MyStruct
    {
        public int x;
    }
}
