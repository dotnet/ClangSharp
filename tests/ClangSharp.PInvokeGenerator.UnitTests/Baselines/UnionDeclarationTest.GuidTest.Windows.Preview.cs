using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [StructLayout(LayoutKind.Explicit)]
    [Guid("00000000-0000-0000-C000-000000000046")]
    public partial struct MyUnion1
    {
        [FieldOffset(0)]
        public int x;
    }

    [StructLayout(LayoutKind.Explicit)]
    [Guid("00000000-0000-0000-C000-000000000047")]
    public partial struct MyUnion2
    {
        [FieldOffset(0)]
        public int x;
    }

    public static partial class Methods
    {
        public static readonly Guid IID_MyUnion1 = new Guid(0x00000000, 0x0000, 0x0000, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46);

        public static readonly Guid IID_MyUnion2 = new Guid(0x00000000, 0x0000, 0x0000, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x47);
    }
}
