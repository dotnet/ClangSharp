using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        public float value;
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe partial struct MyUnion
    {
        [FieldOffset(0)]
        public float r;

        [FieldOffset(0)]
        public float g;

        [FieldOffset(0)]
        public float b;

        [FieldOffset(0)]
        [NativeTypeName("__AnonymousRecord_ClangUnsavedFile_L11_C5")]
        public _Anonymous_e__Union Anonymous;

        public ref float a
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

        public ref float buffer
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
            public float a;

            [FieldOffset(0)]
            public MyStruct s;

            [FieldOffset(0)]
            [NativeTypeName("float[4]")]
            public fixed float buffer[4];
        }
    }
}
