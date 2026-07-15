using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [Guid("9F81F860-3900-421A-A231-E7522F9D7F4A")]
    public partial struct FileOpenDialog
    {
        public int x;
    }

    public static partial class Methods
    {
        public static readonly Guid CLSID_FileOpenDialog = new Guid(0x9F81F860, 0x3900, 0x421A, 0xA2, 0x31, 0xE7, 0x52, 0x2F, 0x9D, 0x7F, 0x4A);
    }
}
