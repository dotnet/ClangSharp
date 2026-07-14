using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public enum MyEnum
    {
        MyEnum_Value,
    }

    public partial struct MyStruct
    {
        public int value;
    }

    public static unsafe partial class Methods
    {
        [DllImport("ClangSharpPInvokeGenerator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "?MyFunction@@YAXP6AXH@Z@Z", ExactSpelling = true)]
        public static extern void MyFunction([NativeTypeName("MyCallback")] delegate* unmanaged[Cdecl]<int, void> callback);
    }
}
