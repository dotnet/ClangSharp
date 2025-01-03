using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public partial struct MyUnion1
    {
        [FieldOffset(0)]
        public uint _bitfield1;

        [NativeTypeName("unsigned int : 24")]
        public uint o0_b0_24
        {
            readonly get
            {
                return _bitfield1 & 0xFFFFFFu;
            }

            set
            {
                _bitfield1 = (_bitfield1 & ~0xFFFFFFu) | (value & 0xFFFFFFu);
            }
        }

        [FieldOffset(0)]
        public uint _bitfield2;

        [NativeTypeName("unsigned int : 16")]
        public uint o4_b0_16
        {
            readonly get
            {
                return _bitfield2 & 0xFFFFu;
            }

            set
            {
                _bitfield2 = (_bitfield2 & ~0xFFFFu) | (value & 0xFFFFu);
            }
        }

        [NativeTypeName("unsigned int : 3")]
        public uint o4_b16_3
        {
            readonly get
            {
                return (_bitfield2 >> 16) & 0x7u;
            }

            set
            {
                _bitfield2 = (_bitfield2 & ~(0x7u << 16)) | ((value & 0x7u) << 16);
            }
        }

        [NativeTypeName("int : 3")]
        public int o4_b19_3
        {
            readonly get
            {
                return (int)(_bitfield2 << 10) >> 29;
            }

            set
            {
                _bitfield2 = (_bitfield2 & ~(0x7u << 19)) | (uint)((value & 0x7) << 19);
            }
        }

        [FieldOffset(0)]
        public byte _bitfield3;

        [NativeTypeName("unsigned char : 1")]
        public byte o4_b22_1
        {
            readonly get
            {
                return (byte)(_bitfield3 & 0x1u);
            }

            set
            {
                _bitfield3 = (byte)((_bitfield3 & ~0x1u) | (value & 0x1u));
            }
        }

        [FieldOffset(0)]
        public int _bitfield4;

        [NativeTypeName("int : 1")]
        public int o4_b23_1
        {
            readonly get
            {
                return (_bitfield4 << 31) >> 31;
            }

            set
            {
                _bitfield4 = (_bitfield4 & ~0x1) | (value & 0x1);
            }
        }

        [NativeTypeName("int : 1")]
        public int o4_b24_1
        {
            readonly get
            {
                return (_bitfield4 << 30) >> 31;
            }

            set
            {
                _bitfield4 = (_bitfield4 & ~(0x1 << 1)) | ((value & 0x1) << 1);
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public partial struct MyUnion2
    {
        [FieldOffset(0)]
        public uint _bitfield1;

        [NativeTypeName("unsigned int : 1")]
        public uint o0_b0_1
        {
            readonly get
            {
                return _bitfield1 & 0x1u;
            }

            set
            {
                _bitfield1 = (_bitfield1 & ~0x1u) | (value & 0x1u);
            }
        }

        [FieldOffset(0)]
        public int x;

        [FieldOffset(0)]
        public uint _bitfield2;

        [NativeTypeName("unsigned int : 1")]
        public uint o8_b0_1
        {
            readonly get
            {
                return _bitfield2 & 0x1u;
            }

            set
            {
                _bitfield2 = (_bitfield2 & ~0x1u) | (value & 0x1u);
            }
        }
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public partial struct MyUnion3
    {
        [FieldOffset(0)]
        public uint _bitfield;

        [NativeTypeName("unsigned int : 1")]
        public uint o0_b0_1
        {
            readonly get
            {
                return _bitfield & 0x1u;
            }

            set
            {
                _bitfield = (_bitfield & ~0x1u) | (value & 0x1u);
            }
        }

        [NativeTypeName("unsigned int : 1")]
        public uint o0_b1_1
        {
            readonly get
            {
                return (_bitfield >> 1) & 0x1u;
            }

            set
            {
                _bitfield = (_bitfield & ~(0x1u << 1)) | ((value & 0x1u) << 1);
            }
        }
    }
}
