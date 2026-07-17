namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        [NativeBitfield("o0_b0_1", offset: 0, length: 1)]
        public uint _bitfield1;

        [NativeTypeName("unsigned int : 1")]
        public uint o0_b0_1
        {
            get
            {
                return _bitfield1 & 0x1u;
            }

            set
            {
                _bitfield1 = (_bitfield1 & ~0x1u) | (value & 0x1u);
            }
        }

        [NativeBitfield("o4_b0_1", offset: 0, length: 1)]
        public uint _bitfield2;

        [NativeTypeName("unsigned int : 1")]
        public uint o4_b0_1
        {
            get
            {
                return _bitfield2 & 0x1u;
            }

            set
            {
                _bitfield2 = (_bitfield2 & ~0x1u) | (value & 0x1u);
            }
        }
    }
}
