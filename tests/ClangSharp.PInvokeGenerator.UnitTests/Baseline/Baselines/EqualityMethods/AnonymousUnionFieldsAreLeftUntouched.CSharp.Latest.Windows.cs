using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public partial struct Outer
    {
        public int Id;

        [NativeTypeName("__AnonymousRecord_ClangUnsavedFile_L4_C5")]
        public _Anonymous_e__Union Anonymous;

        [UnscopedRef]
        public ref int AsInt
        {
            get
            {
                return ref Anonymous.AsInt;
            }
        }

        [UnscopedRef]
        public ref float AsFloat
        {
            get
            {
                return ref Anonymous.AsFloat;
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        public partial struct _Anonymous_e__Union
        {
            [FieldOffset(0)]
            public int AsInt;

            [FieldOffset(0)]
            public float AsFloat;
        }
    }
}
