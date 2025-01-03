using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [NativeTypeName("struct MyStruct : MyTemplate<int>")]
    public unsafe partial struct MyStruct
    {
        [DllImport("ClangSharpPInvokeGenerator", CallingConvention = CallingConvention.ThisCall, EntryPoint = "?DoWork@?$MyTemplate@H@@QEAAPEAHPEAH@Z", ExactSpelling = true)]
        public static extern int* DoWork(MyStruct* pThis, int* value = null);
    }
}
