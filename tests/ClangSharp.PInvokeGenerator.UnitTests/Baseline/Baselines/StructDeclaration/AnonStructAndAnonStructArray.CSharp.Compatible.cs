namespace ClangSharp.Test
{
    public unsafe partial struct _MyStruct
    {
        [NativeTypeName("__AnonymousRecord_ClangUnsavedFile_L3_C5")]
        public _Anonymous_e__Struct Anonymous;

        [NativeTypeName("struct (anonymous struct at ClangUnsavedFile.h:4:5)[2]")]
        public _MyArray_e__FixedBuffer MyArray;

        public ref int First
        {
            get
            {
                fixed (_Anonymous_e__Struct* pField = &Anonymous)
                {
                    return ref pField->First;
                }
            }
        }

        public partial struct _Anonymous_e__Struct
        {
            public int First;
        }

        public partial struct _MyArray_e__Struct
        {
            public int Second;
        }

        public partial struct _MyArray_e__FixedBuffer
        {
            public _MyArray_e__Struct e0;
            public _MyArray_e__Struct e1;

            public unsafe ref _MyArray_e__Struct this[int index]
            {
                get
                {
                    fixed (_MyArray_e__Struct* pThis = &e0)
                    {
                        return ref pThis[index];
                    }
                }
            }
        }
    }
}
