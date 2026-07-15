using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [Guid("42F85136-DB7E-439C-85F1-E4075D135FC8")]
    public partial struct IFileDialog
    {
        public int x;
    }

    public static partial class Methods
    {
        public static readonly Guid IID_IFileDialog = new Guid(0x42F85136, 0xDB7E, 0x439C, 0x85, 0xF1, 0xE4, 0x07, 0x5D, 0x13, 0x5F, 0xC8);
    }
}
