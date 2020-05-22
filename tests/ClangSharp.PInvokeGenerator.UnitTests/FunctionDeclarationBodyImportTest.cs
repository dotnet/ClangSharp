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
        public async Task BreakTest()
        {
            var inputContents = @"int MyFunction(int value)
{
    while (true)
    {
        break;
    }

    return 0;
}
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public static partial class Methods
    {
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

        public static int MyFunction(int value)
        {
            while (true)
            {
                break;
            }

            return 0;
        }
    }
}
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
        public async Task CaseTest()
        {
            var inputContents = @"int MyFunction(int value)
{
    switch (value)
    {
        case 0:
        {
            return 0;
        }

        case 1:
        case 2:
        {
            return 3;
        }
    }

    return -1;
}
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public static partial class Methods
    {
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

        public static int MyFunction(int value)
        {
            switch (value)
            {
                case 0:
                {
                    return 0;
                }

                case 1:
                case 2:
                {
                    return 3;
                }
            }

            return -1;
        }
    }
}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task CaseNoCompoundTest()
        {
            var inputContents = @"int MyFunction(int value)
{
    switch (value)
    {
        case 0:
            return 0;

        case 2:
        case 3:
            return 5;
    }

    return -1;
}
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public static partial class Methods
    {
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

        public static int MyFunction(int value)
        {
            switch (value)
            {
                case 0:
                    return 0;

                case 2:
                case 3:
                    return 5;
            }

            return -1;
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
        public async Task ConditionalOperatorTest()
        {
            var inputContents = @"int MyFunction(bool condition, int lhs, int rhs)
{
    return condition ? lhs : rhs;
}
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public static partial class Methods
    {
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

        public static int MyFunction(bool condition, int lhs, int rhs)
        {
            return condition ? lhs : rhs;
        }
    }
}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task ContinueTest()
        {
            var inputContents = @"int MyFunction(int value)
{
    while (true)
    {
        continue;
    }

    return 0;
}
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public static partial class Methods
    {
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

        public static int MyFunction(int value)
        {
            while (true)
            {
                continue;
            }

            return 0;
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
        public Vtbl* lpVtbl;

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
        public Vtbl* lpVtbl;

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
        public async Task DeclTest()
        {
            var inputContents = @"\
int MyFunction()
{
    int x = 0;
    int y = 1, z = 2;
    return x + y + z;
}
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public static partial class Methods
    {
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

        public static int MyFunction()
        {
            int x = 0;
            int y = 1, z = 2;

            return x + y + z;
        }
    }
}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task DoTest()
        {
            var inputContents = @"int MyFunction(int count)
{
    int i = 0;

    do
    {
        i++;
    }
    while (i < count);

    return i;
}
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public static partial class Methods
    {
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

        public static int MyFunction(int count)
        {
            int i = 0;

            do
            {
                i++;
            }
            while (i < count);

            return i;
        }
    }
}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task DoNonCompoundTest()
        {
            var inputContents = @"int MyFunction(int count)
{
    int i = 0;

    while (i < count)
        i++;

    return i;
}
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public static partial class Methods
    {
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

        public static int MyFunction(int count)
        {
            int i = 0;

            while (i < count)
                i++;

            return i;
        }
    }
}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task ForTest()
        {
            var inputContents = @"int MyFunction(int count)
{
    for (int i = 0; i < count; i--)
    {
        i += 2;
    }

    int x;

    for (x = 0; x < count; x--)
    {
        x += 2;
    }

    x = 0;

    for (; x < count; x--)
    {
        x += 2;
    }

    for (int i = 0;;i--)
    {
        i += 2;
    }

    for (x = 0;;x--)
    {
        x += 2;
    }

    for (int i = 0; i < count;)
    {
        i++;
    }

    for (x = 0; x < count;)
    {
        x++;
    }

    // x = 0;
    // 
    // for (;; x--)
    // {
    //     x += 2;
    // }

    x = 0;

    for (; x < count;)
    {
        x++;
    }

    for (int i = 0;;)
    {
        i++;
    }

    for (x = 0;;)
    {
        x++;
    }

    for (;;)
    {
        return -1;
    }
}
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public static partial class Methods
    {
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

        public static int MyFunction(int count)
        {
            for (int i = 0; i < count; i--)
            {
                i += 2;
            }

            int x;

            for (x = 0; x < count; x--)
            {
                x += 2;
            }

            x = 0;
            for (; x < count; x--)
            {
                x += 2;
            }

            for (int i = 0;; i--)
            {
                i += 2;
            }

            for (x = 0;; x--)
            {
                x += 2;
            }

            for (int i = 0; i < count;)
            {
                i++;
            }

            for (x = 0; x < count;)
            {
                x++;
            }

            x = 0;
            for (; x < count;)
            {
                x++;
            }

            for (int i = 0;;)
            {
                i++;
            }

            for (x = 0;;)
            {
                x++;
            }

            for (;;)
            {
                return -1;
            }
        }
    }
}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task ForNonCompoundTest()
        {
            var inputContents = @"int MyFunction(int count)
{
    for (int i = 0; i < count; i--)
        i += 2;

    int x;

    for (x = 0; x < count; x--)
        x += 2;

    x = 0;

    for (; x < count; x--)
        x += 2;

    for (int i = 0;;i--)
        i += 2;

    for (x = 0;;x--)
        x += 2;

    for (int i = 0; i < count;)
        i++;

    for (x = 0; x < count;)
        x++;

    // x = 0;
    // 
    // for (;; x--)
    //     x += 2;

    x = 0;

    for (; x < count;)
        x++;

    for (int i = 0;;)
        i++;

    for (x = 0;;)
        x++;

    for (;;)
        return -1;
}
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public static partial class Methods
    {
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

        public static int MyFunction(int count)
        {
            for (int i = 0; i < count; i--)
                i += 2;

            int x;

            for (x = 0; x < count; x--)
                x += 2;

            x = 0;
            for (; x < count; x--)
                x += 2;

            for (int i = 0;; i--)
                i += 2;

            for (x = 0;; x--)
                x += 2;

            for (int i = 0; i < count;)
                i++;

            for (x = 0; x < count;)
                x++;

            x = 0;
            for (; x < count;)
                x++;

            for (int i = 0;;)
                i++;

            for (x = 0;;)
                x++;

            for (;;)
                return -1;
        }
    }
}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task IfTest()
        {
            var inputContents = @"int MyFunction(bool condition, int lhs, int rhs)
{
    if (condition)
    {
        return lhs;
    }

    return rhs;
}
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public static partial class Methods
    {
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

        public static int MyFunction(bool condition, int lhs, int rhs)
        {
            if (condition)
            {
                return lhs;
            }

            return rhs;
        }
    }
}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task IfElseTest()
        {
            var inputContents = @"int MyFunction(bool condition, int lhs, int rhs)
{
    if (condition)
    {
        return lhs;
    }
    else
    {
        return rhs;
    }
}
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public static partial class Methods
    {
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

        public static int MyFunction(bool condition, int lhs, int rhs)
        {
            if (condition)
            {
                return lhs;
            }
            else
            {
                return rhs;
            }
        }
    }
}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task IfElseIfTest()
        {
            var inputContents = @"int MyFunction(bool condition1, int a, int b, bool condition2, int c)
{
    if (condition1)
    {
        return a;
    }
    else if (condition2)
    {
        return b;
    }

    return c;
}
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public static partial class Methods
    {
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

        public static int MyFunction(bool condition1, int a, int b, bool condition2, int c)
        {
            if (condition1)
            {
                return a;
            }
            else if (condition2)
            {
                return b;
            }

            return c;
        }
    }
}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task IfElseNonCompoundTest()
        {
            var inputContents = @"int MyFunction(bool condition, int lhs, int rhs)
{
    if (condition)
        return lhs;
    else
        return rhs;
}
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public static partial class Methods
    {
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

        public static int MyFunction(bool condition, int lhs, int rhs)
        {
            if (condition)
                return lhs;
            else
                return rhs;
        }
    }
}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task InitListForArrayTest()
        {
            var inputContents = @"
void MyFunction()
{
    int x[4] = { 1, 2, 3, 4 };
    int y[4] = { 1, 2, 3 };
    int z[] = { 1, 2 };
}
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public static partial class Methods
    {
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

        public static void MyFunction()
        {
            int[] x = new int[4]
            {
                1,
                2,
                3,
                4,
            };
            int[] y = new int[4]
            {
                1,
                2,
                3,
                default,
            };
            int[] z = new int[2]
            {
                1,
                2,
            };
        }
    }
}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task InitListForRecordDeclTest()
        {
            var inputContents = @"struct MyStruct
{
    float x;
    float y;
    float z;
    float w;
};

MyStruct MyFunction1()
{
    return { 1.0f, 2.0f, 3.0f, 4.0f };
}

MyStruct MyFunction2()
{
    return { 1.0f, 2.0f, 3.0f };
}
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        public float x;

        public float y;

        public float z;

        public float w;
    }

    public static partial class Methods
    {
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

        public static MyStruct MyFunction1()
        {
            return new MyStruct
            {
                x = 1.0f,
                y = 2.0f,
                z = 3.0f,
                w = 4.0f,
            };
        }

        public static MyStruct MyFunction2()
        {
            return new MyStruct
            {
                x = 1.0f,
                y = 2.0f,
                z = 3.0f,
            };
        }
    }
}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task MemberTest()
        {
            var inputContents = @"struct MyStruct
{
    int value;
};

int MyFunction1(MyStruct instance)
{
    return instance.value;
}

int MyFunction2(MyStruct* instance)
{
    return instance->value;
}
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        public int value;
    }

    public static unsafe partial class Methods
    {
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

        public static int MyFunction1(MyStruct instance)
        {
            return instance.value;
        }

        public static int MyFunction2([NativeTypeName(""MyStruct *"")] MyStruct* instance)
        {
            return instance->value;
        }
    }
}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task RefToPtrTest()
        {
            var inputContents = @"struct MyStruct {
    int value;
};

bool MyFunction(const MyStruct& lhs, const MyStruct& rhs)
{
    return lhs.value == rhs.value;
}
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        public int value;
    }

    public static unsafe partial class Methods
    {
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

        public static bool MyFunction([NativeTypeName(""const MyStruct &"")] MyStruct* lhs, [NativeTypeName(""const MyStruct &"")] MyStruct* rhs)
        {
            return lhs->value == rhs->value;
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
        public async Task ReturnEmptyTest()
        {
            var inputContents = @"void MyFunction()
{
    return;
}
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public static partial class Methods
    {
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

        public static void MyFunction()
        {
            return;
        }
    }
}
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
        public async Task SwitchTest()
        {
            var inputContents = @"int MyFunction(int value)
{
    switch (value)
    {
        default:
        {
            return 0;
        }
    }
}
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public static partial class Methods
    {
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

        public static int MyFunction(int value)
        {
            switch (value)
            {
                default:
                {
                    return 0;
                }
            }
        }
    }
}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task SwitchNonCompoundTest()
        {
            var inputContents = @"int MyFunction(int value)
{
    switch (value)
        default:
        {
            return 0;
        }

    switch (value)
        default:
            return 0;
}
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public static partial class Methods
    {
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

        public static int MyFunction(int value)
        {
            switch (value)
            {
                default:
                {
                    return 0;
                }
            }

            switch (value)
            {
                default:
                    return 0;
            }
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

        [Fact]
        public async Task WhileTest()
        {
            var inputContents = @"int MyFunction(int count)
{
    int i = 0;

    while (i < count)
    {
        i++;
    }

    return i;
}
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public static partial class Methods
    {
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

        public static int MyFunction(int count)
        {
            int i = 0;

            while (i < count)
            {
                i++;
            }

            return i;
        }
    }
}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task WhileNonCompoundTest()
        {
            var inputContents = @"int MyFunction(int count)
{
    int i = 0;

    while (i < count)
        i++;

    return i;
}
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public static partial class Methods
    {
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

        public static int MyFunction(int count)
        {
            int i = 0;

            while (i < count)
                i++;

            return i;
        }
    }
}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }
    }
}
