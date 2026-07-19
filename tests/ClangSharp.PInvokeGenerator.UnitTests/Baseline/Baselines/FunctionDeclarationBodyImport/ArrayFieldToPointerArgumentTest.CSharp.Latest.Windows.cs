using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        [NativeTypeName("unsigned char[16]")]
        public _Data_e__FixedBuffer Data;

        [InlineArray(16)]
        public partial struct _Data_e__FixedBuffer
        {
            public byte e0;
        }
    }

    public static unsafe partial class Methods
    {
        [DllImport("ClangSharpPInvokeGenerator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "?MyOtherFunction@@YAXPEAX@Z", ExactSpelling = true)]
        public static extern void MyOtherFunction(void* pData);

        public static void MyFunction([NativeTypeName("struct MyStruct *")] MyStruct* pStruct)
        {
            MyOtherFunction(&pStruct->Data);
        }
    }
}
