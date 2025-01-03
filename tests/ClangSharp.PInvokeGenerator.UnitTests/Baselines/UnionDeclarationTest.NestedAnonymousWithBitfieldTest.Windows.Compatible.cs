using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [StructLayout(LayoutKind.Explicit)]
    public unsafe partial struct MyUnion
    {
        [FieldOffset(0)]
        public int x;

        [FieldOffset(0)]
        public int y;

        [FieldOffset(0)]
        [NativeTypeName("__AnonymousRecord_ClangUnsavedFile_L6_C5")]
        public _Anonymous_e__Union Anonymous;

        public ref int z
        {
            get
            {
                fixed (_Anonymous_e__Union* pField = &Anonymous)
                {
                    return ref pField->z;
                }
            }
        }

        public ref int w
        {
            get
            {
                fixed (_Anonymous_e__Union._Anonymous_e__Union* pField = &Anonymous.Anonymous)
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

        [StructLayout(LayoutKind.Explicit)]
        public unsafe partial struct _Anonymous_e__Union
        {
            [FieldOffset(0)]
            public int z;

            [FieldOffset(0)]
            [NativeTypeName("__AnonymousRecord_ClangUnsavedFile_L10_C9")]
            public _Anonymous_e__Union Anonymous;

            [StructLayout(LayoutKind.Explicit)]
            public partial struct _Anonymous_e__Union
            {
                [FieldOffset(0)]
                public int w;

                [FieldOffset(0)]
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
