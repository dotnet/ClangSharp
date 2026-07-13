namespace ClangSharp.Test
{
    public unsafe partial struct _MyStruct
    {
        [NativeTypeName("__AnonymousRecord_ClangUnsavedFile_L3_C5")]
        public _Anonymous_e__Struct Anonymous;

        public ref int Value1
        {
            get
            {
                fixed (_Anonymous_e__Struct._Anonymous1_e__Struct._Anonymous2_e__Struct* pField = &Anonymous.Anonymous1.Anonymous2)
                {
                    return ref pField->Value1;
                }
            }
        }

        public ref int Value2
        {
            get
            {
                fixed (_Anonymous_e__Struct._Anonymous1_e__Struct._Anonymous3_e__Struct* pField = &Anonymous.Anonymous1.Anonymous3)
                {
                    return ref pField->Value2;
                }
            }
        }

        public unsafe partial struct _Anonymous_e__Struct
        {
            [NativeTypeName("__AnonymousRecord_ClangUnsavedFile_L3_C14")]
            public _Anonymous1_e__Struct Anonymous1;

            public unsafe partial struct _Anonymous1_e__Struct
            {
                [NativeTypeName("__AnonymousRecord_ClangUnsavedFile_L4_C9")]
                public _Anonymous2_e__Struct Anonymous2;

                [NativeTypeName("__AnonymousRecord_ClangUnsavedFile_L5_C9")]
                public _Anonymous3_e__Struct Anonymous3;

                public partial struct _Anonymous2_e__Struct
                {
                    public int Value1;
                }

                public partial struct _Anonymous3_e__Struct
                {
                    public int Value2;
                }
            }
        }
    }
}
