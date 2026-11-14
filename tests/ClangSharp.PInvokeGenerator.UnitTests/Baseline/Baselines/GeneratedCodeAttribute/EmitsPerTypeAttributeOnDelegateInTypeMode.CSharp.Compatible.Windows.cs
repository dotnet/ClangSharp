using System;
using System.CodeDom.Compiler;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [GeneratedCode("ClangSharp", "{ClangSharpVersion}")]
    public enum MyEnum
    {
        MyEnum_Value,
    }

    [GeneratedCode("ClangSharp", "{ClangSharpVersion}")]
    public partial struct MyStruct
    {
        public int value;
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [GeneratedCode("ClangSharp", "{ClangSharpVersion}")]
    public delegate void MyCallback(int value);

    [GeneratedCode("ClangSharp", "{ClangSharpVersion}")]
    public static partial class Methods
    {
        [DllImport("ClangSharpPInvokeGenerator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "?MyFunction@@YAXP6AXH@Z@Z", ExactSpelling = true)]
        public static extern void MyFunction([NativeTypeName("MyCallback")] IntPtr callback);
    }
}
