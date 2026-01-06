// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

[Platform("unix")]
public sealed class CSharpCompatibleUnix_CXXMethodDeclarationTest : CXXMethodDeclarationCSharpTest
{
    protected override Task DefaultParameterInheritedFromTemplateTestImpl()
    {
        // NOTE: This is a regression test where a struct inherits a function from a template with a default parameter.
        const string InputContents = @"template <typename T>
struct MyTemplate
{
    int* DoWork(int* value = nullptr)
    {
        return value;
    }
};

struct MyStruct : public MyTemplate<int>
{};
";

        var entryPoint = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "__ZN8$MyTemplateDoWorkEv" : "_ZN10MyTemplateIiE6DoWorkEPi";

        var expectedOutputContents = $@"using System.Runtime.InteropServices;

namespace ClangSharp.Test
{{
    [NativeTypeName(""struct MyStruct : MyTemplate<int>"")]
    public unsafe partial struct MyStruct
    {{
        [DllImport(""ClangSharpPInvokeGenerator"", CallingConvention = CallingConvention.ThisCall, EntryPoint = ""{entryPoint}"", ExactSpelling = true)]
        public static extern int* DoWork(MyStruct* pThis, int* value = null);
    }}
}}
";

        return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(InputContents, expectedOutputContents);
    }

    protected override Task InstanceTestImpl()
    {
        var inputContents = @"struct MyStruct
{
    void MyVoidMethod();

    int MyInt32Method()
    {
        return 0;
    }

    void* MyVoidStarMethod()
    {
        return nullptr;
    }
};
";
        var entryPoint = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "__ZN8MyStruct12MyVoidMethodEv" : "_ZN8MyStruct12MyVoidMethodEv";

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            if (!Environment.Is64BitProcess)
            {
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
    public unsafe partial struct MyStruct
    {{
        [DllImport(""ClangSharpPInvokeGenerator"", CallingConvention = CallingConvention.ThisCall, EntryPoint = ""{entryPoint}"", ExactSpelling = true)]
        public static extern void MyVoidMethod(MyStruct* pThis);

        public int MyInt32Method()
        {{
            return 0;
        }}

        public void* MyVoidStarMethod()
        {{
            return null;
        }}
    }}
}}
";

        return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task NewKeywordVirtualTestImpl()
    {
        var inputContents = @"struct MyStruct
{
    virtual int GetType(int obj) = 0;
    virtual int GetType() = 0;
    virtual int GetType(int objA, int objB) = 0;
};";

        var expectedOutputContents = $@"using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{{
    public unsafe partial struct MyStruct
    {{
        public void** lpVtbl;

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int _GetType(MyStruct* pThis, int obj);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int _GetType1(MyStruct* pThis);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int _GetType2(MyStruct* pThis, int objA, int objB);

        public int GetType(int obj)
        {{
            fixed (MyStruct* pThis = &this)
            {{
                return Marshal.GetDelegateForFunctionPointer<_GetType>((IntPtr)(lpVtbl[0]))(pThis, obj);
            }}
        }}

        public new int GetType()
        {{
            fixed (MyStruct* pThis = &this)
            {{
                return Marshal.GetDelegateForFunctionPointer<_GetType1>((IntPtr)(lpVtbl[1]))(pThis);
            }}
        }}

        public int GetType(int objA, int objB)
        {{
            fixed (MyStruct* pThis = &this)
            {{
                return Marshal.GetDelegateForFunctionPointer<_GetType2>((IntPtr)(lpVtbl[2]))(pThis, objA, objB);
            }}
        }}
    }}
}}
";

        return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task NewKeywordVirtualWithExplicitVtblTestImpl()
    {
        var inputContents = @"struct MyStruct
{
    virtual int GetType(int obj) = 0;
    virtual int GetType() = 0;
    virtual int GetType(int objA, int objB) = 0;
};";

        var nativeCallConv = "";

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && !Environment.Is64BitProcess)
        {
            nativeCallConv = " __attribute__((thiscall))";
        }

        var expectedOutputContents = $@"using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{{
    public unsafe partial struct MyStruct
    {{
        public Vtbl* lpVtbl;

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int _GetType(MyStruct* pThis, int obj);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int _GetType1(MyStruct* pThis);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int _GetType2(MyStruct* pThis, int objA, int objB);

        public int GetType(int obj)
        {{
            fixed (MyStruct* pThis = &this)
            {{
                return Marshal.GetDelegateForFunctionPointer<_GetType>(lpVtbl->GetType)(pThis, obj);
            }}
        }}

        public new int GetType()
        {{
            fixed (MyStruct* pThis = &this)
            {{
                return Marshal.GetDelegateForFunctionPointer<_GetType1>(lpVtbl->GetType1)(pThis);
            }}
        }}

        public int GetType(int objA, int objB)
        {{
            fixed (MyStruct* pThis = &this)
            {{
                return Marshal.GetDelegateForFunctionPointer<_GetType2>(lpVtbl->GetType2)(pThis, objA, objB);
            }}
        }}

        public partial struct Vtbl
        {{
            [NativeTypeName(""int (int){nativeCallConv}"")]
            public new IntPtr GetType;

            [NativeTypeName(""int (){nativeCallConv}"")]
            public IntPtr GetType1;

            [NativeTypeName(""int (int, int){nativeCallConv}"")]
            public IntPtr GetType2;
        }}
    }}
}}
";

        return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorConfigurationOptions.GenerateExplicitVtbls);
    }

    protected override Task NewKeywordVirtualWithExplicitVtblAndMarkerInterfaceTestImpl()
    {
        var inputContents = @"struct MyStruct
{
    virtual int GetType(int obj) = 0;
    virtual int GetType() = 0;
    virtual int GetType(int objA, int objB) = 0;
};";

        var nativeCallConv = "";

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && !Environment.Is64BitProcess)
        {
            nativeCallConv = " __attribute__((thiscall))";
        }

        var expectedOutputContents = $@"using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{{
    public unsafe partial struct MyStruct : MyStruct.Interface
    {{
        public Vtbl* lpVtbl;

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int _GetType(MyStruct* pThis, int obj);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int _GetType1(MyStruct* pThis);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int _GetType2(MyStruct* pThis, int objA, int objB);

        public int GetType(int obj)
        {{
            fixed (MyStruct* pThis = &this)
            {{
                return Marshal.GetDelegateForFunctionPointer<_GetType>(lpVtbl->GetType)(pThis, obj);
            }}
        }}

        public new int GetType()
        {{
            fixed (MyStruct* pThis = &this)
            {{
                return Marshal.GetDelegateForFunctionPointer<_GetType1>(lpVtbl->GetType1)(pThis);
            }}
        }}

        public int GetType(int objA, int objB)
        {{
            fixed (MyStruct* pThis = &this)
            {{
                return Marshal.GetDelegateForFunctionPointer<_GetType2>(lpVtbl->GetType2)(pThis, objA, objB);
            }}
        }}

        public partial interface Interface
        {{
            int GetType(int obj);

            int GetType();

            int GetType(int objA, int objB);
        }}

        public partial struct Vtbl
        {{
            [NativeTypeName(""int (int){nativeCallConv}"")]
            public new IntPtr GetType;

            [NativeTypeName(""int (){nativeCallConv}"")]
            public IntPtr GetType1;

            [NativeTypeName(""int (int, int){nativeCallConv}"")]
            public IntPtr GetType2;
        }}
    }}
}}
";

        return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorConfigurationOptions.GenerateExplicitVtbls | PInvokeGeneratorConfigurationOptions.GenerateMarkerInterfaces);
    }

    protected override Task StaticTestImpl()
    {
        var inputContents = @"struct MyStruct
{
    static void MyVoidMethod();

    static int MyInt32Method()
    {
        return 0;
    }

    static void* MyVoidStarMethod()
    {
        return nullptr;
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
    public unsafe partial struct MyStruct
    {{
        [DllImport(""ClangSharpPInvokeGenerator"", CallingConvention = CallingConvention.Cdecl, EntryPoint = ""{entryPoint}"", ExactSpelling = true)]
        public static extern void MyVoidMethod();

        public static int MyInt32Method()
        {{
            return 0;
        }}

        public static void* MyVoidStarMethod()
        {{
            return null;
        }}
    }}
}}
";

        return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task VirtualTestImpl()
    {
        var inputContents = @"struct MyStruct
{
    virtual void MyVoidMethod() = 0;

    virtual signed char MyInt8Method()
    {
        return 0;
    }

    virtual int MyInt32Method();

    virtual void* MyVoidStarMethod() = 0;
};
";

        var expectedOutputContents = $@"using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{{
    public unsafe partial struct MyStruct
    {{
        public void** lpVtbl;

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void _MyVoidMethod(MyStruct* pThis);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        [return: NativeTypeName(""signed char"")]
        public delegate sbyte _MyInt8Method(MyStruct* pThis);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int _MyInt32Method(MyStruct* pThis);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void* _MyVoidStarMethod(MyStruct* pThis);

        public void MyVoidMethod()
        {{
            fixed (MyStruct* pThis = &this)
            {{
                Marshal.GetDelegateForFunctionPointer<_MyVoidMethod>((IntPtr)(lpVtbl[0]))(pThis);
            }}
        }}

        [return: NativeTypeName(""signed char"")]
        public sbyte MyInt8Method()
        {{
            fixed (MyStruct* pThis = &this)
            {{
                return Marshal.GetDelegateForFunctionPointer<_MyInt8Method>((IntPtr)(lpVtbl[1]))(pThis);
            }}
        }}

        public int MyInt32Method()
        {{
            fixed (MyStruct* pThis = &this)
            {{
                return Marshal.GetDelegateForFunctionPointer<_MyInt32Method>((IntPtr)(lpVtbl[2]))(pThis);
            }}
        }}

        public void* MyVoidStarMethod()
        {{
            fixed (MyStruct* pThis = &this)
            {{
                return Marshal.GetDelegateForFunctionPointer<_MyVoidStarMethod>((IntPtr)(lpVtbl[3]))(pThis);
            }}
        }}
    }}
}}
";

        return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task VirtualWithVtblIndexAttributeTestImpl()
    {
        var inputContents = @"struct MyStruct
{
    virtual void MyVoidMethod() = 0;

    virtual signed char MyInt8Method()
    {
        return 0;
    }

    virtual int MyInt32Method();

    virtual void* MyVoidStarMethod() = 0;
};
";

        var expectedOutputContents = $@"using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{{
    public unsafe partial struct MyStruct
    {{
        public void** lpVtbl;

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void _MyVoidMethod(MyStruct* pThis);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        [return: NativeTypeName(""signed char"")]
        public delegate sbyte _MyInt8Method(MyStruct* pThis);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int _MyInt32Method(MyStruct* pThis);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void* _MyVoidStarMethod(MyStruct* pThis);

        [VtblIndex(0)]
        public void MyVoidMethod()
        {{
            fixed (MyStruct* pThis = &this)
            {{
                Marshal.GetDelegateForFunctionPointer<_MyVoidMethod>((IntPtr)(lpVtbl[0]))(pThis);
            }}
        }}

        [VtblIndex(1)]
        [return: NativeTypeName(""signed char"")]
        public sbyte MyInt8Method()
        {{
            fixed (MyStruct* pThis = &this)
            {{
                return Marshal.GetDelegateForFunctionPointer<_MyInt8Method>((IntPtr)(lpVtbl[1]))(pThis);
            }}
        }}

        [VtblIndex(2)]
        public int MyInt32Method()
        {{
            fixed (MyStruct* pThis = &this)
            {{
                return Marshal.GetDelegateForFunctionPointer<_MyInt32Method>((IntPtr)(lpVtbl[2]))(pThis);
            }}
        }}

        [VtblIndex(3)]
        public void* MyVoidStarMethod()
        {{
            fixed (MyStruct* pThis = &this)
            {{
                return Marshal.GetDelegateForFunctionPointer<_MyVoidStarMethod>((IntPtr)(lpVtbl[3]))(pThis);
            }}
        }}
    }}
}}
";

        return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorConfigurationOptions.GenerateVtblIndexAttribute);
    }

    protected override Task ValidateBindingsAsync(string inputContents, string expectedOutputContents) => ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
}
