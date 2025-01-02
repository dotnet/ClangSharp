using System;

namespace ClangSharp.Test
{
    public static partial class Methods
    {
        [NativeTypeName("const GUID")]
        public static readonly Guid IID_IUnknown = new Guid(0x00000000, 0x0000, 0x0000, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46);
    }
}
