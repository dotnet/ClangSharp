using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [Guid("C5BDD8D0-D26E-11CE-A89E-00AA006CADC5")]
    public partial struct ITextHost
    {
        public int x;
    }

    public static partial class Methods
    {
        public static readonly Guid IID_ITextHost = new Guid(0xC5BDD8D0, 0xD26E, 0x11CE, 0xA8, 0x9E, 0x00, 0xAA, 0x00, 0x6C, 0xAD, 0xC5);
    }
}
