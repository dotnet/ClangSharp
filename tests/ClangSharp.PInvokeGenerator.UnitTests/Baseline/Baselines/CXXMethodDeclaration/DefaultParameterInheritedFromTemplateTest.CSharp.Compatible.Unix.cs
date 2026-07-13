using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [NativeTypeName("struct MyStruct : MyTemplate<int>")]
    public unsafe partial struct MyStruct
    {
        [DllImport("ClangSharpPInvokeGenerator", CallingConvention = CallingConvention.ThisCall, EntryPoint = "_ZN10MyTemplateIiE6DoWorkEPi", ExactSpelling = true)]
        public static extern int* DoWork(MyStruct* pThis, int* value = null);
    }
}
