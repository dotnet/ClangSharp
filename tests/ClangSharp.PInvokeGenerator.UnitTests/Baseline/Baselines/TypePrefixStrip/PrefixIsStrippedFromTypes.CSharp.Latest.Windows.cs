using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public partial struct some_struct
    {
        public int value;
    }

    public enum some_enum
    {
        some_enum_key1,
        some_enum_key2,
    }

    public partial struct _2d_point
    {
        public int x;

        public int y;
    }

    public static unsafe partial class Methods
    {
        [DllImport("ClangSharpPInvokeGenerator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "?abc_use@@YAXPEAUabc_some_struct@@W4abc_some_enum@@PEAUabc_2d_point@@@Z", ExactSpelling = true)]
        public static extern void abc_use([NativeTypeName("abc_some_struct *")] some_struct* s, [NativeTypeName("abc_some_enum")] some_enum e, [NativeTypeName("abc_2d_point *")] _2d_point* p);
    }
}
