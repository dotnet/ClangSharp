namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        public byte _bitfield1;

        [NativeTypeName("bool : 1")]
        public bool a
        {
            get
            {
                return (_bitfield1 & 0x1u) != 0;
            }

            set
            {
                _bitfield1 = (byte)((_bitfield1 & ~0x1u) | ((value ? 1 : 0) & 0x1u));
            }
        }

        [NativeTypeName("bool : 1")]
        public bool b
        {
            get
            {
                return ((_bitfield1 >> 1) & 0x1u) != 0;
            }

            set
            {
                _bitfield1 = (byte)((_bitfield1 & ~(0x1u << 1)) | (((value ? 1 : 0) & 0x1u) << 1));
            }
        }

        public int _bitfield2;

        [NativeTypeName("int : 2")]
        public int c
        {
            get
            {
                return (_bitfield2 << 30) >> 30;
            }

            set
            {
                _bitfield2 = (_bitfield2 & ~0x3) | (value & 0x3);
            }
        }
    }
}
