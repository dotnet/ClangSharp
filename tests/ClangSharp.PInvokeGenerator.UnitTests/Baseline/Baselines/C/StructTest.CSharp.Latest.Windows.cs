using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public partial struct _MyStruct
    {
        public int _field;
    }

    public unsafe partial struct _MyOtherStruct
    {
        [NativeTypeName("MyStruct")]
        public _MyStruct _field1;

        [NativeTypeName("MyStruct *")]
        public _MyStruct* _field2;
    }

    public partial struct _MyStructWithAnonymousStruct
    {
        [NativeTypeName("__AnonymousRecord_ClangUnsavedFile_L14_C5")]
        public __anonymousStructField1_e__Struct _anonymousStructField1;

        public partial struct __anonymousStructField1_e__Struct
        {
            public int _field;
        }
    }

    public partial struct _MyStructWithAnonymousUnion
    {
        [NativeTypeName("__AnonymousRecord_ClangUnsavedFile_L21_C5")]
        public _union1_e__Union union1;

        [StructLayout(LayoutKind.Explicit)]
        public unsafe partial struct _union1_e__Union
        {
            [FieldOffset(0)]
            public int _field1;

            [FieldOffset(0)]
            public int* _field2;
        }
    }
}
