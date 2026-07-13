using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace ClangSharp.Test
{
    public partial struct _MyStruct
    {
        [NativeTypeName("__AnonymousRecord_ClangUnsavedFile_L3_C5")]
        public _Anonymous_e__Struct Anonymous;

        [NativeTypeName("struct (anonymous struct at ClangUnsavedFile.h:4:5)[2]")]
        public _MyArray_e__FixedBuffer MyArray;

        [UnscopedRef]
        public ref int First
        {
            get
            {
                return ref Anonymous.First;
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

        [InlineArray(2)]
        public partial struct _MyArray_e__FixedBuffer
        {
            public _MyArray_e__Struct e0;
        }
    }
}
