using System.CodeDom.Compiler;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [GeneratedCode("ClangSharp", "22.1.8.0")]
    public enum MyEnum
    {
        MyEnum_Value,
    }

    [GeneratedCode("ClangSharp", "22.1.8.0")]
    public partial struct MyStruct
    {
        public int value;
    }

    [GeneratedCode("ClangSharp", "22.1.8.0")]
    public static unsafe partial class Methods
    {
        [DllImport("ClangSharpPInvokeGenerator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "?MyFunction@@YAXP6AXH@Z@Z", ExactSpelling = true)]
        public static extern void MyFunction([NativeTypeName("MyCallback")] delegate* unmanaged[Cdecl]<int, void> callback);
    }
}
