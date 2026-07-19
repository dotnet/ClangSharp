using ClangSharp.Test;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System
{
    public partial struct Guid
    {
        [NativeTypeName("unsigned long")]
        public uint Data1;

        [NativeTypeName("unsigned short")]
        public ushort Data2;

        [NativeTypeName("unsigned short")]
        public ushort Data3;

        [NativeTypeName("unsigned char[8]")]
        public _Data4_e__FixedBuffer Data4;

        [InlineArray(8)]
        public partial struct _Data4_e__FixedBuffer
        {
            public byte e0;
        }
    }

    [Guid("E504A81C-6B01-4A88-8E1E-000000000000")]
    public partial struct ITransferTarget
    {
        public int x;
    }

    public static unsafe partial class Methods
    {
        public static ref readonly Guid IID_ITransferTarget
        {
            get
            {
                ReadOnlySpan<byte> data = [
                    0x1C, 0xA8, 0x04, 0xE5,
                    0x01, 0x6B,
                    0x88, 0x4A,
                    0x8E,
                    0x1E,
                    0x00,
                    0x00,
                    0x00,
                    0x00,
                    0x00,
                    0x00
                ];

                Debug.Assert(data.Length == Unsafe.SizeOf<Guid>());
                return ref Unsafe.As<byte, Guid>(ref MemoryMarshal.GetReference(data));
            }
        }
    }
}
