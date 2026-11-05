using System;
using System.CodeDom.Compiler;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [GeneratedCode("ClangSharp", "23.1.5.0")]
    public enum MyEnum
    {
        MyEnum_Value,
    }

    [GeneratedCode("ClangSharp", "23.1.5.0")]
    public partial struct MyStruct
    {
        public int value;
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [GeneratedCode("ClangSharp", "23.1.5.0")]
    public delegate void MyCallback(int value);

    [GeneratedCode("ClangSharp", "23.1.5.0")]
    public static partial class Methods
    {
        [DllImport("ClangSharpPInvokeGenerator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "?MyFunction@@YAXP6AXH@Z@Z", ExactSpelling = true)]
        public static extern void MyFunction([NativeTypeName("MyCallback")] IntPtr callback);
    }
}
