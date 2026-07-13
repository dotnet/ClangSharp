using System.Diagnostics.CodeAnalysis;

namespace ClangSharp.Test
{
    public partial struct _MyStruct
    {
        [NativeTypeName("__AnonymousRecord_ClangUnsavedFile_L3_C5")]
        public _Anonymous_e__Struct Anonymous;

        [UnscopedRef]
        public ref int Value1
        {
            get
            {
                return ref Anonymous.Anonymous1.Anonymous2.Value1;
            }
        }

        [UnscopedRef]
        public ref int Value2
        {
            get
            {
                return ref Anonymous.Anonymous1.Anonymous3.Value2;
            }
        }

        public partial struct _Anonymous_e__Struct
        {
            [NativeTypeName("__AnonymousRecord_ClangUnsavedFile_L3_C14")]
            public _Anonymous1_e__Struct Anonymous1;

            public partial struct _Anonymous1_e__Struct
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
