namespace ClangSharp.Test
{
    public unsafe partial struct MyStruct
    {
        public int x;

        public int y;

        [NativeTypeName("__AnonymousRecord_ClangUnsavedFile_L6_C5")]
        public _Anonymous_e__Struct Anonymous;

        public ref int z
        {
            get
            {
                fixed (_Anonymous_e__Struct* pField = &Anonymous)
                {
                    return ref pField->z;
                }
            }
        }

        public ref int w
        {
            get
            {
                fixed (_Anonymous_e__Struct._Anonymous_e__Struct* pField = &Anonymous.Anonymous)
                {
                    return ref pField->w;
                }
            }
        }

        public int o0_b0_16
        {
            get
            {
                return Anonymous.Anonymous.o0_b0_16;
            }

            set
            {
                Anonymous.Anonymous.o0_b0_16 = value;
            }
        }

        public int o0_b16_4
        {
            get
            {
                return Anonymous.Anonymous.o0_b16_4;
            }

            set
            {
                Anonymous.Anonymous.o0_b16_4 = value;
            }
        }

        public unsafe partial struct _Anonymous_e__Struct
        {
            public int z;

            [NativeTypeName("__AnonymousRecord_ClangUnsavedFile_L10_C9")]
            public _Anonymous_e__Struct Anonymous;

            public partial struct _Anonymous_e__Struct
            {
                public int w;

                public int _bitfield;

                [NativeTypeName("int : 16")]
                public int o0_b0_16
                {
                    get
                    {
                        return (_bitfield << 16) >> 16;
                    }

                    set
                    {
                        _bitfield = (_bitfield & ~0xFFFF) | (value & 0xFFFF);
                    }
                }

                [NativeTypeName("int : 4")]
                public int o0_b16_4
                {
                    get
                    {
                        return (_bitfield << 12) >> 28;
                    }

                    set
                    {
                        _bitfield = (_bitfield & ~(0xF << 16)) | ((value & 0xF) << 16);
                    }
                }
            }
        }
    }
}
