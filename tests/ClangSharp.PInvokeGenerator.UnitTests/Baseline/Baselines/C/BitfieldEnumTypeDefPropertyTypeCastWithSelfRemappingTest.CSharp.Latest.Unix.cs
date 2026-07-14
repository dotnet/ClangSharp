namespace ClangSharp.Test
{
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
        public Flags flags
        {
            readonly get
            {
                return (Flags)((_bitfield >> 8) & 0xFFu);
            }

            set
            {
                _bitfield = (_bitfield & ~(0xFFu << 8)) | (((uint)(value) & 0xFFu) << 8);
            }
        }
    }
}
