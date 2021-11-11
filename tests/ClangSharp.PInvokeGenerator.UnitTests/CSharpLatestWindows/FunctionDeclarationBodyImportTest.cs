// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ClangSharp.UnitTests
{
    public sealed class CSharpLatestWindows_FunctionDeclarationBodyImportTest : FunctionDeclarationBodyImportTest
    {
        public override Task ArraySubscriptTest()
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

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task BasicTest()
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

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task BinaryOperatorBasicTest(string opcode)
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

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task BinaryOperatorCompareTest(string opcode)
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

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task BinaryOperatorBooleanTest(string opcode)
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

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task BreakTest()
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

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task CallFunctionTest()
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

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task CallFunctionWithArgsTest()
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

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task CaseTest()
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

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task CaseNoCompoundTest()
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

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task CompareMultipleEnumTest()
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

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task ConditionalOperatorTest()
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

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task ContinueTest()
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

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task CStyleFunctionalCastTest()
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

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task CxxFunctionalCastTest()
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

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task CxxConstCastTest()
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

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task CxxDynamicCastTest()
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

            var expectedOutputContents = $@"using System.Runtime.CompilerServices;

namespace ClangSharp.Test
{{
    public unsafe partial struct MyStructA
    {{
        public void** lpVtbl;

        public void MyMethod()
        {{
            ((delegate* unmanaged[Thiscall]<MyStructA*, void>)(lpVtbl[0]))((MyStructA*)Unsafe.AsPointer(ref this));
        }}
    }}

    [NativeTypeName(""struct MyStructB : MyStructA"")]
    public unsafe partial struct MyStructB
    {{
        public void** lpVtbl;

        public void MyMethod()
        {{
            ((delegate* unmanaged[Thiscall]<MyStructB*, void>)(lpVtbl[0]))((MyStructB*)Unsafe.AsPointer(ref this));
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

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task CxxReinterpretCastTest()
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

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task CxxStaticCastTest()
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

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task DeclTest()
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

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task DoTest()
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

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task DoNonCompoundTest()
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

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task ForTest()
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

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task ForNonCompoundTest()
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

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task IfTest()
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

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task IfElseTest()
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

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task IfElseIfTest()
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

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task IfElseNonCompoundTest()
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

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task InitListForArrayTest()
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

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task InitListForRecordDeclTest()
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

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task MemberTest()
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

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task RefToPtrTest()
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

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task ReturnCXXNullPtrTest()
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

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task ReturnCXXBooleanLiteralTest(string value)
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

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task ReturnFloatingLiteralDoubleTest(string value)
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

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task ReturnFloatingLiteralSingleTest(string value)
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

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task ReturnEmptyTest()
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

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task ReturnIntegerLiteralInt32Test()
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

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task AccessUnionMemberTest()
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
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.a, 1));
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

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task ReturnStructTest()
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

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task SwitchTest()
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

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task SwitchNonCompoundTest()
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

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task UnaryOperatorAddrOfTest()
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

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task UnaryOperatorDerefTest()
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

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task UnaryOperatorLogicalNotTest()
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

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task UnaryOperatorPostfixTest(string opcode)
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

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task UnaryOperatorPrefixTest(string opcode)
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

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task WhileTest()
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

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task WhileNonCompoundTest()
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

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }
    }
}
