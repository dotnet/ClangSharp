namespace ClangSharp.Test
{
    public unsafe partial struct MyStruct
    {
        public double r;

        public double g;

        public double b;

        [NativeTypeName("__AnonymousRecord_ClangUnsavedFile_L7_C5")]
        public _Anonymous_e__Struct Anonymous;

        public ref double a
        {
            get
            {
                fixed (_Anonymous_e__Struct* pField = &Anonymous)
                {
                    return ref pField->a;
                }
            }
        }

        public partial struct _Anonymous_e__Struct
        {
            public double a;
        }
    }
}
