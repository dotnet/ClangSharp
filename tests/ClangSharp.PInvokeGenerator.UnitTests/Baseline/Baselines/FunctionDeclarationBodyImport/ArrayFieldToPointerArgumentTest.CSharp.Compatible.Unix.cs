using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public unsafe partial struct MyStruct
    {
        [NativeTypeName("unsigned char[16]")]
        public fixed byte Data[16];
    }

    public static unsafe partial class Methods
    {
        [DllImport("ClangSharpPInvokeGenerator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "_Z15MyOtherFunctionPv", ExactSpelling = true)]
        public static extern void MyOtherFunction(void* pData);

        public static void MyFunction([NativeTypeName("struct MyStruct *")] MyStruct* pStruct)
        {
            MyOtherFunction(pStruct->Data);
            pStruct->Data[15] = 1;
        }
    }
}
