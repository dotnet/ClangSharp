using System.Diagnostics.CodeAnalysis;

namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        public int x;

        public int y;

        [NativeTypeName("__AnonymousRecord_ClangUnsavedFile_L6_C5")]
        public _Anonymous_e__Struct Anonymous;

        [UnscopedRef]
        public ref int z
        {
            get
            {
                return ref Anonymous.z;
            }
        }

        [UnscopedRef]
        public ref int w
        {
            get
            {
                return ref Anonymous.Anonymous.w;
            }
        }

        public int o0_b0_16
        {
            readonly get
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
            readonly get
            {
                return Anonymous.Anonymous.o0_b16_4;
            }

            set
            {
                Anonymous.Anonymous.o0_b16_4 = value;
            }
        }

        public partial struct _Anonymous_e__Struct
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
                    readonly get
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
                    readonly get
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
