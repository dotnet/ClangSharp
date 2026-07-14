namespace ClangSharp.Test
{
    public partial struct Bitfield
    {
        public byte _bitfield1;

        [NativeTypeName("unsigned char : 8")]
        public byte bits1
        {
            readonly get
            {
                return (byte)(_bitfield1 & 0xFFu);
            }

            set
            {
                _bitfield1 = (byte)((_bitfield1 & ~0xFFu) | (value & 0xFFu));
            }
        }

        public sbyte _bitfield2;

        [NativeTypeName("char : 8")]
        public sbyte bits2
        {
            readonly get
            {
                return (sbyte)((_bitfield2 << 0) >> 0);
            }

            set
            {
                _bitfield2 = (sbyte)((_bitfield2 & ~0xFF) | (value & 0xFF));
            }
        }

        public byte _bitfield3;

        [NativeTypeName("unsigned char : 8")]
        public byte bits3
        {
            readonly get
            {
                return (byte)(_bitfield3 & 0xFFu);
            }

            set
            {
                _bitfield3 = (byte)((_bitfield3 & ~0xFFu) | (value & 0xFFu));
            }
        }
    }
}
