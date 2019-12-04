// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Xunit;

namespace ClangSharp.UnitTests
{
    public sealed class FunctionDeclarationBodyImportTest : PInvokeGeneratorTest
    {
        [Fact]
        public async Task ArraySubscriptTest()
        {
            var inputContents = @"int MyFunction(int* pData, int index)
{
    return pData[index];
}
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public static unsafe partial class Methods
    {
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

        public static int MyFunction([NativeTypeName(""int *"")] int* pData, int index)
        {
            return pData[index];
        }
    }
}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task BasicTest()
        {
            var inputContents = @"void MyFunction()
{
}
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public static partial class Methods
    {
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

        public static void MyFunction()
        {
        }
    }
}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Theory]
        [InlineData("%")]
        [InlineData("%=")]
        [InlineData("&")]
        [InlineData("&=")]
        [InlineData("*")]
        [InlineData("*=")]
        [InlineData("+")]
        [InlineData("+=")]
        [InlineData("-")]
        [InlineData("-=")]
        [InlineData("/")]
        [InlineData("/=")]
        [InlineData("<<")]
        [InlineData("<<=")]
        [InlineData("=")]
        [InlineData("==")]
        [InlineData(">>")]
        [InlineData(">>=")]
        [InlineData("^")]
        [InlineData("^=")]
        [InlineData("|")]
        [InlineData("|=")]
        public async Task BinaryOperatorBasicTest(string opcode)
        {
            var inputContents = $@"int MyFunction(int x, int y)
{{
    return x {opcode} y;
}}
";

            var expectedOutputContents = $@"namespace ClangSharp.Test
{{
    public static partial class Methods
    {{
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

        public static int MyFunction(int x, int y)
        {{
            return x {opcode} y;
        }}
    }}
}}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Theory]
        [InlineData("!=")]
        [InlineData("&&")]
        [InlineData("<")]
        [InlineData("<=")]
        [InlineData(">")]
        [InlineData(">=")]
        [InlineData("||")]
        public async Task BinaryOperatorBooleanTest(string opcode)
        {
            var inputContents = $@"bool MyFunction(bool x, bool y)
{{
    return x {opcode} y;
}}
";

            var expectedOutputContents = $@"namespace ClangSharp.Test
{{
    public static partial class Methods
    {{
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

        public static bool MyFunction(bool x, bool y)
        {{
            return x {opcode} y;
        }}
    }}
}}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task CallFunctionTest()
        {
            var inputContents = @"void MyCalledFunction()
{
}

void MyFunction()
{
    MyCalledFunction();
}
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public static partial class Methods
    {
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

        public static void MyCalledFunction()
        {
        }

        public static void MyFunction()
        {
            MyCalledFunction();
        }
    }
}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task CallFunctionWithArgsTest()
        {
            var inputContents = @"void MyCalledFunction(int x, int y)
{
}

void MyFunction()
{
    MyCalledFunction(0, 1);
}
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public static partial class Methods
    {
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

        public static void MyCalledFunction(int x, int y)
        {
        }

        public static void MyFunction()
        {
            MyCalledFunction(0, 1);
        }
    }
}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task CompareMultipleEnumTest()
        {
            var inputContents = @"enum MyEnum : int
{
    MyEnum_Value0,
    MyEnum_Value1,
    MyEnum_Value2,
};

static inline int MyFunction(MyEnum x)
{
    return x == MyEnum_Value0 ||
           x == MyEnum_Value1 ||
           x == MyEnum_Value2;
}
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public enum MyEnum
    {
        MyEnum_Value0,
        MyEnum_Value1,
        MyEnum_Value2,
    }

    public static partial class Methods
    {
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

        public static int MyFunction(MyEnum x)
        {
            return x == MyEnum_Value0 || x == MyEnum_Value1 || x == MyEnum_Value2;
        }
    }
}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task CStyleFunctionalCastTest()
        {
            var inputContents = @"int MyFunction(float input)
{
    return (int)input;
}
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public static partial class Methods
    {
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

        public static int MyFunction(float input)
        {
            return (int)input;
        }
    }
}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task CxxFunctionalCastTest()
        {
            var inputContents = @"int MyFunction(float input)
{
    return int(input);
}
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public static partial class Methods
    {
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

        public static int MyFunction(float input)
        {
            return (int)input;
        }
    }
}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task CxxConstCastTest()
        {
            var inputContents = @"void* MyFunction(const void* input)
{
    return const_cast<void*>(input);
}
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public static unsafe partial class Methods
    {
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

        [return: NativeTypeName(""void *"")]
        public static void* MyFunction([NativeTypeName(""const void *"")] void* input)
        {
            return input;
        }
    }
}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task CxxDynamicCastTest()
        {
            var inputContents = @"struct MyStructA
{
    virtual void MyMethod() = 0;
};

struct MyStructB : MyStructA { };

MyStructB* MyFunction(MyStructA* input)
{
    return dynamic_cast<MyStructB*>(input);
}
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
    public unsafe partial struct MyStructA
    {{
        public readonly Vtbl* lpVtbl;

        [UnmanagedFunctionPointer(CallingConvention.{callConv})]
        public delegate void _MyMethod(MyStructA* pThis);

        public void MyMethod()
        {{
            Marshal.GetDelegateForFunctionPointer<_MyMethod>(lpVtbl->MyMethod)((MyStructA*)Unsafe.AsPointer(ref this));
        }}

        public partial struct Vtbl
        {{
            [NativeTypeName(""void (){callConvAttr}"")]
            public IntPtr MyMethod;
        }}
    }}

    public unsafe partial struct MyStructB
    {{
        public readonly Vtbl* lpVtbl;

        [UnmanagedFunctionPointer(CallingConvention.{callConv})]
        public delegate void _MyMethod(MyStructB* pThis);

        public void MyMethod()
        {{
            Marshal.GetDelegateForFunctionPointer<_MyMethod>(lpVtbl->MyMethod)((MyStructB*)Unsafe.AsPointer(ref this));
        }}

        public partial struct Vtbl
        {{
            [NativeTypeName(""void (){callConvAttr}"")]
            public IntPtr MyMethod;
        }}
    }}

    public static unsafe partial class Methods
    {{
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

        [return: NativeTypeName(""MyStructB *"")]
        public static MyStructB* MyFunction([NativeTypeName(""MyStructA *"")] MyStructA* input)
        {{
            return (MyStructB*)input;
        }}
    }}
}}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task CxxReinterpretCastTest()
        {
            var inputContents = @"int* MyFunction(void* input)
{
    return reinterpret_cast<int*>(input);
}
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public static unsafe partial class Methods
    {
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

        [return: NativeTypeName(""int *"")]
        public static int* MyFunction([NativeTypeName(""void *"")] void* input)
        {
            return (int*)input;
        }
    }
}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task CxxStaticCastTest()
        {
            var inputContents = @"int* MyFunction(void* input)
{
    return static_cast<int*>(input);
}
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public static unsafe partial class Methods
    {
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

        [return: NativeTypeName(""int *"")]
        public static int* MyFunction([NativeTypeName(""void *"")] void* input)
        {
            return (int*)input;
        }
    }
}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task ReturnCXXNullPtrTest()
        {
            var inputContents = @"void* MyFunction()
{
    return nullptr;
}
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public static unsafe partial class Methods
    {
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

        [return: NativeTypeName(""void *"")]
        public static void* MyFunction()
        {
            return null;
        }
    }
}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Theory]
        [InlineData("false")]
        [InlineData("true")]
        public async Task ReturnCXXBooleanLiteralTest(string value)
        {
            var inputContents = $@"bool MyFunction()
{{
    return {value};
}}
";

            var expectedOutputContents = $@"namespace ClangSharp.Test
{{
    public static partial class Methods
    {{
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

        public static bool MyFunction()
        {{
            return {value};
        }}
    }}
}}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Theory]
        [InlineData("5e-1")]
        [InlineData("3.14")]
        public async Task ReturnFloatingLiteralDoubleTest(string value)
        {
            var inputContents = $@"double MyFunction()
{{
    return {value};
}}
";

            var expectedOutputContents = $@"namespace ClangSharp.Test
{{
    public static partial class Methods
    {{
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

        public static double MyFunction()
        {{
            return {value};
        }}
    }}
}}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Theory]
        [InlineData("5e-1f")]
        [InlineData("3.14f")]
        public async Task ReturnFloatingLiteralSingleTest(string value)
        {
            var inputContents = $@"float MyFunction()
{{
    return {value};
}}
";

            var expectedOutputContents = $@"namespace ClangSharp.Test
{{
    public static partial class Methods
    {{
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

        public static float MyFunction()
        {{
            return {value};
        }}
    }}
}}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task ReturnIntegerLiteralInt32Test()
        {
            var inputContents = @"int MyFunction()
{
    return -1;
}
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public static partial class Methods
    {
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

        public static int MyFunction()
        {
            return -1;
        }
    }
}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task UnaryOperatorAddrOfTest()
        {
            var inputContents = @"int* MyFunction(int value)
{
    return &value;
}
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public static unsafe partial class Methods
    {
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

        [return: NativeTypeName(""int *"")]
        public static int* MyFunction(int value)
        {
            return &value;
        }
    }
}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task UnaryOperatorDerefTest()
        {
            var inputContents = @"int MyFunction(int* value)
{
    return *value;
}
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public static unsafe partial class Methods
    {
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

        public static int MyFunction([NativeTypeName(""int *"")] int* value)
        {
            return *value;
        }
    }
}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task UnaryOperatorLogicalNotTest()
        {
            var inputContents = @"bool MyFunction(bool value)
{
    return !value;
}
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public static partial class Methods
    {
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

        public static bool MyFunction(bool value)
        {
            return !value;
        }
    }
}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Theory]
        [InlineData("++")]
        [InlineData("--")]
        public async Task UnaryOperatorPostfixTest(string opcode)
        {
            var inputContents = $@"int MyFunction(int value)
{{
    return value{opcode};
}}
";

            var expectedOutputContents = $@"namespace ClangSharp.Test
{{
    public static partial class Methods
    {{
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

        public static int MyFunction(int value)
        {{
            return value{opcode};
        }}
    }}
}}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Theory]
        [InlineData("+")]
        [InlineData("++")]
        [InlineData("-")]
        [InlineData("--")]
        [InlineData("~")]
        public async Task UnaryOperatorPrefixTest(string opcode)
        {
            var inputContents = $@"int MyFunction(int value)
{{
    return {opcode}value;
}}
";

            var expectedOutputContents = $@"namespace ClangSharp.Test
{{
    public static partial class Methods
    {{
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

        public static int MyFunction(int value)
        {{
            return {opcode}value;
        }}
    }}
}}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }
    }
}
