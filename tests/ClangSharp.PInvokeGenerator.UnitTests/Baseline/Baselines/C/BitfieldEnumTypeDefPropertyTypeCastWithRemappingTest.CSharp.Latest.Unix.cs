namespace ClangSharp.Test
{
    [NativeTypeName("unsigned int")]
    public enum FlagBits : uint
    {
        Member = 0x7FFFFFFF,
    }

    public partial struct Bitfield
    {
        public uint _bitfield;

        [NativeTypeName("unsigned int : 8")]
        public uint bits
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

        [NativeTypeName("Flags : 8")]
        public FlagBits flags
        {
            readonly get
            {
                return (FlagBits)((_bitfield >> 8) & 0xFFu);
            }

            set
            {
                _bitfield = (_bitfield & ~(0xFFu << 8)) | (((uint)(value) & 0xFFu) << 8);
            }
        }
    }
}
