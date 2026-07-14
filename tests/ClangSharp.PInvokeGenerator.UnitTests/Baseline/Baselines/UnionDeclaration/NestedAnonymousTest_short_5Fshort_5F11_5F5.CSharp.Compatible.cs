using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        public short value;
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe partial struct MyUnion
    {
        [FieldOffset(0)]
        public short r;

        [FieldOffset(0)]
        public short g;

        [FieldOffset(0)]
        public short b;

        [FieldOffset(0)]
        [NativeTypeName("__AnonymousRecord_ClangUnsavedFile_L11_C5")]
        public _Anonymous_e__Union Anonymous;

        public ref short a
        {
            get
            {
                fixed (_Anonymous_e__Union* pField = &Anonymous)
                {
                    return ref pField->a;
                }
            }
        }

        public ref MyStruct s
        {
            get
            {
                fixed (_Anonymous_e__Union* pField = &Anonymous)
                {
                    return ref pField->s;
                }
            }
        }

        public ref short buffer
        {
            get
            {
                fixed (_Anonymous_e__Union* pField = &Anonymous)
                {
                    return ref pField->buffer[0];
                }
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        public unsafe partial struct _Anonymous_e__Union
        {
            [FieldOffset(0)]
            public short a;

            [FieldOffset(0)]
            public MyStruct s;

            [FieldOffset(0)]
            [NativeTypeName("short[4]")]
            public fixed short buffer[4];
        }
    }
}
