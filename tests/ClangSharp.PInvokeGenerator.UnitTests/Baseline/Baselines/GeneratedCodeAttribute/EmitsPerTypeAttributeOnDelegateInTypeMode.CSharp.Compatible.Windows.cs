using System;
using System.CodeDom.Compiler;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [GeneratedCode("ClangSharp", "21.1.8.4")]
    public enum MyEnum
    {
        MyEnum_Value,
    }

    [GeneratedCode("ClangSharp", "21.1.8.4")]
    public partial struct MyStruct
    {
        public int value;
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [GeneratedCode("ClangSharp", "21.1.8.4")]
    public delegate void MyCallback(int value);

    [GeneratedCode("ClangSharp", "21.1.8.4")]
    public static partial class Methods
    {
        [DllImport("ClangSharpPInvokeGenerator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "?MyFunction@@YAXP6AXH@Z@Z", ExactSpelling = true)]
        public static extern void MyFunction([NativeTypeName("MyCallback")] IntPtr callback);
    }
}
