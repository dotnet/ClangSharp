// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ClangSharp.UnitTests;

public sealed class CSharpCompatibleUnix_FunctionDeclarationBodyImportTest : FunctionDeclarationBodyImportTest
{
    protected override Task ArraySubscriptTestImpl()
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
        public static int MyFunction(int* pData, int index)
        {
            return pData[index];
        }
    }
}
";

        return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task BasicTestImpl()
    {
        var inputContents = @"void MyFunction()
{
}
";

        var expectedOutputContents = @"namespace ClangSharp.Test
{
    public static partial class Methods
    {
        public static void MyFunction()
        {
        }
    }
}
";

        return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task BinaryOperatorBasicTestImpl(string opcode)
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
        public static int MyFunction(int x, int y)
        {{
            return x {opcode} y;
        }}
    }}
}}
";

        return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task BinaryOperatorCompareTestImpl(string opcode)
    {
        var inputContents = $@"bool MyFunction(int x, int y)
{{
    return x {opcode} y;
}}
";

        var expectedOutputContents = $@"namespace ClangSharp.Test
{{
    public static partial class Methods
    {{
        public static bool MyFunction(int x, int y)
        {{
            return x {opcode} y;
        }}
    }}
}}
";

        return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task BinaryOperatorBooleanTestImpl(string opcode)
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
        public static bool MyFunction(bool x, bool y)
        {{
            return x {opcode} y;
        }}
    }}
}}
";

        return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task BreakTestImpl()
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

        return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task CallFunctionTestImpl()
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

        return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task CallFunctionWithArgsTestImpl()
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

        return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task CaseTestImpl()
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

        return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task CaseNoCompoundTestImpl()
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
        public static int MyFunction(int value)
        {
            switch (value)
            {
                case 0:
                {
                    return 0;
                }

                case 2:
                case 3:
                {
                    return 5;
                }
            }

            return -1;
        }
    }
}
";

        return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task CompareMultipleEnumTestImpl()
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

        var expectedOutputContents = @"using static ClangSharp.Test.MyEnum;

namespace ClangSharp.Test
{
    public enum MyEnum
    {
        MyEnum_Value0,
        MyEnum_Value1,
        MyEnum_Value2,
    }

    public static partial class Methods
    {
        public static int MyFunction(MyEnum x)
        {
            return (x == MyEnum_Value0 || x == MyEnum_Value1 || x == MyEnum_Value2) ? 1 : 0;
        }
    }
}
";

        return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task ConditionalOperatorTestImpl()
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
        public static int MyFunction(bool condition, int lhs, int rhs)
        {
            return condition ? lhs : rhs;
        }
    }
}
";

        return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task ContinueTestImpl()
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

        return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task CStyleFunctionalCastTestImpl()
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
        public static int MyFunction(float input)
        {
            return (int)(input);
        }
    }
}
";

        return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task CxxFunctionalCastTestImpl()
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
        public static int MyFunction(float input)
        {
            return (int)(input);
        }
    }
}
";

        return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task CxxConstCastTestImpl()
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
        public static void* MyFunction([NativeTypeName(""const void *"")] void* input)
        {
            return input;
        }
    }
}
";

        return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task CxxDynamicCastTestImpl()
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

        var expectedOutputContents = $@"using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{{
    public unsafe partial struct MyStructA
    {{
        public void** lpVtbl;

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void _MyMethod(MyStructA* pThis);

        public void MyMethod()
        {{
            fixed (MyStructA* pThis = &this)
            {{
                Marshal.GetDelegateForFunctionPointer<_MyMethod>((IntPtr)(lpVtbl[0]))(pThis);
            }}
        }}
    }}

    [NativeTypeName(""struct MyStructB : MyStructA"")]
    public unsafe partial struct MyStructB
    {{
        public void** lpVtbl;

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void _MyMethod(MyStructB* pThis);

        public void MyMethod()
        {{
            fixed (MyStructB* pThis = &this)
            {{
                Marshal.GetDelegateForFunctionPointer<_MyMethod>((IntPtr)(lpVtbl[0]))(pThis);
            }}
        }}
    }}

    public static unsafe partial class Methods
    {{
        public static MyStructB* MyFunction(MyStructA* input)
        {{
            return (MyStructB*)(input);
        }}
    }}
}}
";

        return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task CxxReinterpretCastTestImpl()
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
        public static int* MyFunction(void* input)
        {
            return (int*)(input);
        }
    }
}
";

        return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task CxxStaticCastTestImpl()
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
        public static int* MyFunction(void* input)
        {
            return (int*)(input);
        }
    }
}
";

        return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task DeclTestImpl()
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
        public static int MyFunction()
        {
            int x = 0;
            int y = 1, z = 2;

            return x + y + z;
        }
    }
}
";

        return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task DoTestImpl()
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

        return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task DoNonCompoundTestImpl()
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

        return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task ForTestImpl()
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

        return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task ForNonCompoundTestImpl()
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

        return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task IfTestImpl()
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

        return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task IfElseTestImpl()
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

        return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task IfElseIfTestImpl()
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

        return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task IfElseNonCompoundTestImpl()
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

        return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task InitListForArrayTestImpl()
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

        return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task InitListForRecordDeclTestImpl()
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

        return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task MemberTestImpl()
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
        public static int MyFunction1(MyStruct instance)
        {
            return instance.value;
        }

        public static int MyFunction2(MyStruct* instance)
        {
            return instance->value;
        }
    }
}
";

        return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task RefToPtrTestImpl()
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
        public static bool MyFunction([NativeTypeName(""const MyStruct &"")] MyStruct* lhs, [NativeTypeName(""const MyStruct &"")] MyStruct* rhs)
        {
            return lhs->value == rhs->value;
        }
    }
}
";

        return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task ReturnCXXNullPtrTestImpl()
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
        public static void* MyFunction()
        {
            return null;
        }
    }
}
";

        return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task ReturnCXXBooleanLiteralTestImpl(string value)
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
        public static bool MyFunction()
        {{
            return {value};
        }}
    }}
}}
";

        return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task ReturnFloatingLiteralDoubleTestImpl(string value)
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
        public static double MyFunction()
        {{
            return {value};
        }}
    }}
}}
";

        return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task ReturnFloatingLiteralSingleTestImpl(string value)
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
        public static float MyFunction()
        {{
            return {value};
        }}
    }}
}}
";

        return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task ReturnEmptyTestImpl()
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
        public static void MyFunction()
        {
            return;
        }
    }
}
";

        return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task ReturnIntegerLiteralInt32TestImpl()
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
        public static int MyFunction()
        {
            return -1;
        }
    }
}
";

        return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task AccessUnionMemberTestImpl()
    {
        var inputContents = @"union MyUnion
{
    struct { int a; };
};

void MyFunction()
{
    MyUnion myUnion;  
    myUnion.a = 10;
}
";

        var expectedOutputContents = @"using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct MyUnion
    {
        [FieldOffset(0)]
        [NativeTypeName(""MyUnion::(anonymous struct at ClangUnsavedFile.h:3:5)"")]
        public _Anonymous_e__Struct Anonymous;

        public ref int a
        {
            get
            {
                fixed (_Anonymous_e__Struct* pField = &Anonymous)
                {
                    return ref pField->a;
                }
            }
        }

        public partial struct _Anonymous_e__Struct
        {
            public int a;
        }
    }

    public static partial class Methods
    {
        public static void MyFunction()
        {
            MyUnion myUnion = new MyUnion();

            myUnion.Anonymous.a = 10;
        }
    }
}
";

        return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task ReturnStructTestImpl()
    {
        var inputContents = @"struct MyStruct
{
    double r;
    double g;
    double b;
};

MyStruct MyFunction()
{
    MyStruct myStruct;
    return myStruct;
}
";

        var expectedOutputContents = @"namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        public double r;

        public double g;

        public double b;
    }

    public static partial class Methods
    {
        public static MyStruct MyFunction()
        {
            MyStruct myStruct = new MyStruct();

            return myStruct;
        }
    }
}
";

        return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task SwitchTestImpl()
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

        return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task SwitchNonCompoundTestImpl()
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
                {
                    return 0;
                }
            }
        }
    }
}
";

        return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task UnaryOperatorAddrOfTestImpl()
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
        public static int* MyFunction(int value)
        {
            return &value;
        }
    }
}
";

        return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task UnaryOperatorDerefTestImpl()
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
        public static int MyFunction(int* value)
        {
            return *value;
        }
    }
}
";

        return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task UnaryOperatorLogicalNotTestImpl()
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
        public static bool MyFunction(bool value)
        {
            return !value;
        }
    }
}
";

        return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task UnaryOperatorPostfixTestImpl(string opcode)
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
        public static int MyFunction(int value)
        {{
            return value{opcode};
        }}
    }}
}}
";

        return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task UnaryOperatorPrefixTestImpl(string opcode)
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
        public static int MyFunction(int value)
        {{
            return {opcode}value;
        }}
    }}
}}
";

        return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task WhileTestImpl()
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

        return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task WhileNonCompoundTestImpl()
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

        return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }
}
