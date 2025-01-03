using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public partial struct MyStruct
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
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.a, 1));
            }
        }

        public partial struct _Anonymous_e__Struct
        {
            public double a;
        }
    }
}
