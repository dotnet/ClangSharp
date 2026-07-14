using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct MyUnion
    {
        [FieldOffset(0)]
        public int value;
    }

    public unsafe partial struct MyStruct
    {
        public int x;

        public int y;

        [NativeTypeName("__AnonymousRecord_ClangUnsavedFile_L10_C5")]
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

        public ref _Anonymous_e__Struct._w_e__Struct w
        {
            get
            {
                fixed (_Anonymous_e__Struct* pField = &Anonymous)
                {
                    return ref pField->w;
                }
            }
        }

        public ref int value1
        {
            get
            {
                fixed (_Anonymous_e__Struct._Anonymous1_e__Struct* pField = &Anonymous.Anonymous1)
                {
                    return ref pField->value1;
                }
            }
        }

        public ref int value
        {
            get
            {
                fixed (_Anonymous_e__Struct._Anonymous1_e__Struct._Anonymous_e__Struct* pField = &Anonymous.Anonymous1.Anonymous)
                {
                    return ref pField->value;
                }
            }
        }

        public ref int value2
        {
            get
            {
                fixed (_Anonymous_e__Struct._Anonymous2_e__Union* pField = &Anonymous.Anonymous2)
                {
                    return ref pField->value2;
                }
            }
        }

        public ref MyUnion u
        {
            get
            {
                fixed (_Anonymous_e__Struct* pField = &Anonymous)
                {
                    return ref pField->u;
                }
            }
        }

        public ref int buffer1
        {
            get
            {
                fixed (_Anonymous_e__Struct* pField = &Anonymous)
                {
                    return ref pField->buffer1[0];
                }
            }
        }

        public ref _Anonymous_e__Struct._buffer2_e__FixedBuffer buffer2
        {
            get
            {
                fixed (_Anonymous_e__Struct* pField = &Anonymous)
                {
                    return ref pField->buffer2;
                }
            }
        }

        public unsafe partial struct _Anonymous_e__Struct
        {
            public int z;

            [NativeTypeName("__AnonymousRecord_ClangUnsavedFile_L14_C9")]
            public _w_e__Struct w;

            [NativeTypeName("__AnonymousRecord_ClangUnsavedFile_L19_C9")]
            public _Anonymous1_e__Struct Anonymous1;

            [NativeTypeName("__AnonymousRecord_ClangUnsavedFile_L29_C9")]
            public _Anonymous2_e__Union Anonymous2;

            public MyUnion u;

            [NativeTypeName("int[4]")]
            public fixed int buffer1[4];

            [NativeTypeName("MyUnion[4]")]
            public _buffer2_e__FixedBuffer buffer2;

            public partial struct _w_e__Struct
            {
                public int value;
            }

            public unsafe partial struct _Anonymous1_e__Struct
            {
                public int value1;

                [NativeTypeName("__AnonymousRecord_ClangUnsavedFile_L23_C13")]
                public _Anonymous_e__Struct Anonymous;

                public partial struct _Anonymous_e__Struct
                {
                    public int value;
                }
            }

            [StructLayout(LayoutKind.Explicit)]
            public partial struct _Anonymous2_e__Union
            {
                [FieldOffset(0)]
                public int value2;
            }

            public partial struct _buffer2_e__FixedBuffer
            {
                public MyUnion e0;
                public MyUnion e1;
                public MyUnion e2;
                public MyUnion e3;

                public unsafe ref MyUnion this[int index]
                {
                    get
                    {
                        fixed (MyUnion* pThis = &e0)
                        {
                            return ref pThis[index];
                        }
                    }
                }
            }
        }
    }
}
