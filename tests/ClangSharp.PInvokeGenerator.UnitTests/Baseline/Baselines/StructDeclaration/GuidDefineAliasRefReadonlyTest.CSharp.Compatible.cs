using ClangSharp.Test;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System
{
    public unsafe partial struct Guid
    {
        [NativeTypeName("unsigned long")]
        public uint Data1;

        [NativeTypeName("unsigned short")]
        public ushort Data2;

        [NativeTypeName("unsigned short")]
        public ushort Data3;

        [NativeTypeName("unsigned char[8]")]
        public fixed byte Data4[8];
    }

    [Guid("79EAC9E0-BAF9-11CE-8C82-00AA004BA90B")]
    public partial struct IInternet
    {
        public int x;
    }

    public static unsafe partial class Methods
    {
        [NativeTypeName("#define IID_IOInet IID_IInternet")]
        public static ref readonly Guid IID_IOInet => ref IID_IInternet;

        [NativeTypeName("#define DIPROP_BUFFERSIZE (*(const GUID *)(1))")]
        public static Guid* DIPROP_BUFFERSIZE => unchecked((Guid*)(1));

        public static ref readonly Guid IID_IInternet
        {
            get
            {
                ReadOnlySpan<byte> data = new byte[] {
                    0xE0, 0xC9, 0xEA, 0x79,
                    0xF9, 0xBA,
                    0xCE, 0x11,
                    0x8C,
                    0x82,
                    0x00,
                    0xAA,
                    0x00,
                    0x4B,
                    0xA9,
                    0x0B
                };

                Debug.Assert(data.Length == Unsafe.SizeOf<Guid>());
                return ref Unsafe.As<byte, Guid>(ref MemoryMarshal.GetReference(data));
            }
        }
    }
}
