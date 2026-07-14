using System.CodeDom.Compiler;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [GeneratedCode("ClangSharp", "21.1.8.3")]
    public enum MyEnum
    {
        MyEnum_Value,
    }

    [GeneratedCode("ClangSharp", "21.1.8.3")]
    public partial struct MyStruct
    {
        public int value;
    }

    [GeneratedCode("ClangSharp", "21.1.8.3")]
    public static unsafe partial class Methods
    {
        [DllImport("ClangSharpPInvokeGenerator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "?MyFunction@@YAXP6AXH@Z@Z", ExactSpelling = true)]
        public static extern void MyFunction([NativeTypeName("MyCallback")] delegate* unmanaged[Cdecl]<int, void> callback);
    }
}
