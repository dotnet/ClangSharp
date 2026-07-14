namespace ClangSharp.Test
{
    public partial struct Bitfield
    {
        public uint _bitfield;

        [NativeTypeName("Number : 8")]
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
    }
}
