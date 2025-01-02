using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct MyUnion
    {
        [FieldOffset(0)]
        [NativeTypeName("__AnonymousRecord_ClangUnsavedFile_L3_C5")]
        public _Anonymous_e__Struct Anonymous;

        [UnscopedRef]
        public ref int a
        {
            get
            {
                return ref Anonymous.a;
            }
        }

        public partial struct _Anonymous_e__Struct
        {
            public int a;
        }
    }

    public static partial class Methods
    {
        public static void MyFunction()
        {
            MyUnion myUnion = new MyUnion();

            myUnion.Anonymous.a = 10;
        }
    }
}
