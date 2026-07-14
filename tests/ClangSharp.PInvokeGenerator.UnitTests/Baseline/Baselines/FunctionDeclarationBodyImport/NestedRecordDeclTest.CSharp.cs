using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        public int x;
    }

    public static partial class Methods
    {
        public static int MyFunction()
        {
            _MyFunction_e__Union u = new _MyFunction_e__Union();

            u.i = 0;
            return u.s.x;
        }

        [StructLayout(LayoutKind.Explicit)]
        public partial struct _MyFunction_e__Union
        {
            [FieldOffset(0)]
            [NativeTypeName("struct MyStruct")]
            public MyStruct s;

            [FieldOffset(0)]
            public int i;
        }
    }
}
