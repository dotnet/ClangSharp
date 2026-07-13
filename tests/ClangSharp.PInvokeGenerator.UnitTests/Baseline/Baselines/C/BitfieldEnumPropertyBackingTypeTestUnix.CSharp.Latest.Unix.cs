namespace ClangSharp.Test
{
    public partial struct IntBitfield
    {
        public int _bitfield;

        [NativeTypeName("int : 8")]
        public int bits
        {
            readonly get
            {
                return (_bitfield << 24) >> 24;
            }

            set
            {
                _bitfield = (_bitfield & ~0xFF) | (value & 0xFF);
            }
        }

        [NativeTypeName("unsigned int : 8")]
        public uint bits2
        {
            readonly get
            {
                return (uint)((_bitfield >> 8) & 0xFF);
            }

            set
            {
                _bitfield = (_bitfield & ~(0xFF << 8)) | (int)((value & 0xFFu) << 8);
            }
        }
    }

    public partial struct UIntBitfield
    {
        public uint _bitfield;

        [NativeTypeName("unsigned int : 8")]
        public uint bits1
        {
            readonly get
            {
                return _bitfield & 0xFFu;
            }

            set
            {
                _bitfield = (_bitfield & ~0xFFu) | (value & 0xFFu);
            }
        }

        [NativeTypeName("int : 8")]
        public int bits2
        {
            readonly get
            {
                return (int)(_bitfield << 16) >> 24;
            }

            set
            {
                _bitfield = (_bitfield & ~(0xFFu << 8)) | (uint)((value & 0xFF) << 8);
            }
        }

        [NativeTypeName("unsigned char : 8")]
        public byte bits3
        {
            readonly get
            {
                return (byte)((_bitfield >> 16) & 0xFFu);
            }

            set
            {
                _bitfield = (_bitfield & ~(0xFFu << 16)) | (uint)((value & 0xFFu) << 16);
            }
        }

        [NativeTypeName("char : 8")]
        public sbyte bits4
        {
            readonly get
            {
                return (sbyte)((sbyte)(_bitfield << 0) >> 24);
            }

            set
            {
                _bitfield = (_bitfield & ~(0xFFu << 24)) | (uint)((value & 0xFF) << 24);
            }
        }
    }
}
