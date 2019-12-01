// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Xunit;

namespace ClangSharp.UnitTests
{
    public sealed class CXXMethodDeclarationTest : PInvokeGeneratorTest
    {
        [Fact]
        public async Task InstanceTest()
        {
            var inputContents = @"struct MyStruct
{
    void MyVoidMethod();

    int MyInt32Method()
    {
        return 0;
    }
};
";
            var callConv = "Cdecl";
            var entryPoint = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "__ZN8MyStruct12MyVoidMethodEv" : "_ZN8MyStruct12MyVoidMethodEv";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (!Environment.Is64BitProcess)
                {
                    callConv = "ThisCall";
                    entryPoint = "?MyVoidMethod@MyStruct@@QAEXXZ";
                }
                else
                {
                    entryPoint = "?MyVoidMethod@MyStruct@@QEAAXXZ";
                }
            }

            var expectedOutputContents = $@"using System.Runtime.InteropServices;

namespace ClangSharp.Test
{{
    public partial struct MyStruct
    {{
        [DllImport(Methods.LibraryPath, CallingConvention = CallingConvention.{callConv}, EntryPoint = ""{entryPoint}"", ExactSpelling = true)]
        public static extern void MyVoidMethod();

        public int MyInt32Method()
        {{
            return 0;
        }}
    }}
}}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task StaticTest()
        {
            var inputContents = @"struct MyStruct
{
    static void MyVoidMethod();

    static int MyInt32Method()
    {
        return 0;
    }
};
";

            var entryPoint = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "?MyVoidMethod@MyStruct@@SAXXZ" : "_ZN8MyStruct12MyVoidMethodEv";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                entryPoint = $"_{entryPoint}";
            }

            var expectedOutputContents = $@"using System.Runtime.InteropServices;

namespace ClangSharp.Test
{{
    public partial struct MyStruct
    {{
        [DllImport(Methods.LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = ""{entryPoint}"", ExactSpelling = true)]
        public static extern void MyVoidMethod();

        public static int MyInt32Method()
        {{
            return 0;
        }}
    }}
}}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task VirtualCompatibleTest()
        {
            var inputContents = @"struct MyStruct
{
    virtual void MyVoidMethod() = 0;

    virtual char MyInt8Method()
    {
        return 0;
    }

    virtual int MyInt32Method();
};
";

            var callConv = "Cdecl";
            var callConvAttr = "";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && !Environment.Is64BitProcess)
            {
                callConv = "ThisCall";
                callConvAttr = " __attribute__((thiscall))";
            }

            var expectedOutputContents = $@"using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{{
    public unsafe partial struct MyStruct
    {{
        public readonly Vtbl* lpVtbl;

        [UnmanagedFunctionPointer(CallingConvention.{callConv})]
        public delegate void _MyVoidMethod(MyStruct* pThis);

        [UnmanagedFunctionPointer(CallingConvention.{callConv})]
        [return: NativeTypeName(""char"")]
        public delegate sbyte _MyInt8Method(MyStruct* pThis);

        [UnmanagedFunctionPointer(CallingConvention.{callConv})]
        public delegate int _MyInt32Method(MyStruct* pThis);

        public void MyVoidMethod()
        {{
            fixed (MyStruct* pThis = &this)
            {{
                Marshal.GetDelegateForFunctionPointer<_MyVoidMethod>(lpVtbl->MyVoidMethod)(pThis);
            }}
        }}

        [return: NativeTypeName(""char"")]
        public sbyte MyInt8Method()
        {{
            fixed (MyStruct* pThis = &this)
            {{
                return Marshal.GetDelegateForFunctionPointer<_MyInt8Method>(lpVtbl->MyInt8Method)(pThis);
            }}
        }}

        public int MyInt32Method()
        {{
            fixed (MyStruct* pThis = &this)
            {{
                return Marshal.GetDelegateForFunctionPointer<_MyInt32Method>(lpVtbl->MyInt32Method)(pThis);
            }}
        }}

        public partial struct Vtbl
        {{
            [NativeTypeName(""void (){callConvAttr}"")]
            public IntPtr MyVoidMethod;

            [NativeTypeName(""char (){callConvAttr}"")]
            public IntPtr MyInt8Method;

            [NativeTypeName(""int (){callConvAttr}"")]
            public IntPtr MyInt32Method;
        }}
    }}
}}
";

            await ValidateGeneratedCompatibleBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task VirtualTest()
        {
            var inputContents = @"struct MyStruct
{
    virtual void MyVoidMethod() = 0;

    virtual char MyInt8Method()
    {
        return 0;
    }

    virtual int MyInt32Method();
};
";

            var callConv = "Cdecl";
            var callConvAttr = "";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && !Environment.Is64BitProcess)
            {
                callConv = "ThisCall";
                callConvAttr = " __attribute__((thiscall))";
            }

            var expectedOutputContents = $@"using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{{
    public unsafe partial struct MyStruct
    {{
        public readonly Vtbl* lpVtbl;

        [UnmanagedFunctionPointer(CallingConvention.{callConv})]
        public delegate void _MyVoidMethod(MyStruct* pThis);

        [UnmanagedFunctionPointer(CallingConvention.{callConv})]
        [return: NativeTypeName(""char"")]
        public delegate sbyte _MyInt8Method(MyStruct* pThis);

        [UnmanagedFunctionPointer(CallingConvention.{callConv})]
        public delegate int _MyInt32Method(MyStruct* pThis);

        public void MyVoidMethod()
        {{
            Marshal.GetDelegateForFunctionPointer<_MyVoidMethod>(lpVtbl->MyVoidMethod)((MyStruct*)Unsafe.AsPointer(ref this));
        }}

        [return: NativeTypeName(""char"")]
        public sbyte MyInt8Method()
        {{
            return Marshal.GetDelegateForFunctionPointer<_MyInt8Method>(lpVtbl->MyInt8Method)((MyStruct*)Unsafe.AsPointer(ref this));
        }}

        public int MyInt32Method()
        {{
            return Marshal.GetDelegateForFunctionPointer<_MyInt32Method>(lpVtbl->MyInt32Method)((MyStruct*)Unsafe.AsPointer(ref this));
        }}

        public partial struct Vtbl
        {{
            [NativeTypeName(""void (){callConvAttr}"")]
            public IntPtr MyVoidMethod;

            [NativeTypeName(""char (){callConvAttr}"")]
            public IntPtr MyInt8Method;

            [NativeTypeName(""int (){callConvAttr}"")]
            public IntPtr MyInt32Method;
        }}
    }}
}}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }
    }
}
