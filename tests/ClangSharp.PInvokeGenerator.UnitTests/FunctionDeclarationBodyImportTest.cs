// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

[TestFixtureSource(nameof(FixtureArgs))]
public sealed class FunctionDeclarationBodyImportTest(PInvokeGeneratorOutputMode outputMode, PInvokeGeneratorConfigurationOptions outputVersion)
    : PInvokeGeneratorTest(outputMode, outputVersion)
{
    [Test]
    public Task ArraySubscriptTest()
    {
        var inputContents = @"int MyFunction(int* pData, int index)
{
    return pData[index];
}
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task BasicTest()
    {
        var inputContents = @"void MyFunction()
{
}
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [TestCase("%")]
    [TestCase("%=")]
    [TestCase("&")]
    [TestCase("&=")]
    [TestCase("*")]
    [TestCase("*=")]
    [TestCase("+")]
    [TestCase("+=")]
    [TestCase("-")]
    [TestCase("-=")]
    [TestCase("/")]
    [TestCase("/=")]
    [TestCase("<<")]
    [TestCase("<<=")]
    [TestCase("=")]
    [TestCase(">>")]
    [TestCase(">>=")]
    [TestCase("^")]
    [TestCase("^=")]
    [TestCase("|")]
    [TestCase("|=")]
    public Task BinaryOperatorBasicTest(string opcode)
    {
        var inputContents = $@"int MyFunction(int x, int y)
{{
    return x {opcode} y;
}}
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [TestCase("==")]
    [TestCase("!=")]
    [TestCase("<")]
    [TestCase("<=")]
    [TestCase(">")]
    [TestCase(">=")]
    public Task BinaryOperatorCompareTest(string opcode)
    {
        var inputContents = $@"bool MyFunction(int x, int y)
{{
    return x {opcode} y;
}}
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [TestCase("&&")]
    [TestCase("||")]
    public Task BinaryOperatorBooleanTest(string opcode)
    {
        var inputContents = $@"bool MyFunction(bool x, bool y)
{{
    return x {opcode} y;
}}
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task BreakTest()
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

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task CallFunctionTest()
    {
        var inputContents = @"void MyCalledFunction()
{
}

void MyFunction()
{
    MyCalledFunction();
}
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task CallFunctionWithArgsTest()
    {
        var inputContents = @"void MyCalledFunction(int x, int y)
{
}

void MyFunction()
{
    MyCalledFunction(0, 1);
}
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task CaseTest()
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

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task CaseNoCompoundTest()
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

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task CompareMultipleEnumTest()
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

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task ConditionalOperatorTest()
    {
        var inputContents = @"int MyFunction(bool condition, int lhs, int rhs)
{
    return condition ? lhs : rhs;
}
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task ContinueTest()
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

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task CStyleFunctionalCastTest()
    {
        var inputContents = @"int MyFunction(float input)
{
    return (int)input;
}
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task CxxFunctionalCastTest()
    {
        var inputContents = @"int MyFunction(float input)
{
    return int(input);
}
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task CxxConstCastTest()
    {
        var inputContents = @"void* MyFunction(const void* input)
{
    return const_cast<void*>(input);
}
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task CxxDynamicCastTest()
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

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task CxxReinterpretCastTest()
    {
        var inputContents = @"int* MyFunction(void* input)
{
    return reinterpret_cast<int*>(input);
}
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task CxxStaticCastTest()
    {
        var inputContents = @"int* MyFunction(void* input)
{
    return static_cast<int*>(input);
}
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task DeclTest()
    {
        var inputContents = @"\
int MyFunction()
{
    int x = 0;
    int y = 1, z = 2;
    return x + y + z;
}
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task DoTest()
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

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task DoNonCompoundTest()
    {
        var inputContents = @"int MyFunction(int count)
{
    int i = 0;

    while (i < count)
        i++;

    return i;
}
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task ForTest()
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

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task ForNonCompoundTest()
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

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task IfTest()
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

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task IfElseTest()
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

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task IfElseIfTest()
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

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task IfElseNonCompoundTest()
    {
        var inputContents = @"int MyFunction(bool condition, int lhs, int rhs)
{
    if (condition)
        return lhs;
    else
        return rhs;
}
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task InitListForArrayTest()
    {
        var inputContents = @"
void MyFunction()
{
    int x[4] = { 1, 2, 3, 4 };
    int y[4] = { 1, 2, 3 };
    int z[] = { 1, 2 };
}
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task InitListForRecordDeclTest()
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

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task MemberTest()
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

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task RefToPtrTest()
    {
        var inputContents = @"struct MyStruct {
    int value;
};

bool MyFunction(const MyStruct& lhs, const MyStruct& rhs)
{
    return lhs.value == rhs.value;
}
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task ReturnCXXNullPtrTest()
    {
        var inputContents = @"void* MyFunction()
{
    return nullptr;
}
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [TestCase("false")]
    [TestCase("true")]
    public Task ReturnCXXBooleanLiteralTest(string value)
    {
        var inputContents = $@"bool MyFunction()
{{
    return {value};
}}
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [TestCase("5e-1f")]
    [TestCase("3.14f")]
    public Task ReturnFloatingLiteralDoubleTest(string value)
    {
        var inputContents = $@"double MyFunction()
{{
    return {value};
}}
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [TestCase("5e-1f")]
    [TestCase("3.14f")]
    public Task ReturnFloatingLiteralSingleTest(string value)
    {
        var inputContents = $@"float MyFunction()
{{
    return {value};
}}
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task ReturnEmptyTest()
    {
        var inputContents = @"void MyFunction()
{
    return;
}
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task ReturnIntegerLiteralInt32Test()
    {
        var inputContents = @"int MyFunction()
{
    return -1;
}
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task AccessUnionMemberTest()
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

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task ReturnStructTest()
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

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task SwitchTest()
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

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task SwitchNonCompoundTest()
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

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task UnaryOperatorAddrOfTest()
    {
        var inputContents = @"int* MyFunction(int value)
{
    return &value;
}
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task UnaryOperatorDerefTest()
    {
        var inputContents = @"int MyFunction(int* value)
{
    return *value;
}
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task UnaryOperatorLogicalNotTest()
    {
        var inputContents = @"bool MyFunction(bool value)
{
    return !value;
}
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [TestCase("++")]
    [TestCase("--")]
    public Task UnaryOperatorPostfixTest(string opcode)
    {
        var inputContents = $@"int MyFunction(int value)
{{
    return value{opcode};
}}
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [TestCase("+")]
    [TestCase("++")]
    [TestCase("-")]
    [TestCase("--")]
    [TestCase("~")]
    public Task UnaryOperatorPrefixTest(string opcode)
    {
        var inputContents = $@"int MyFunction(int value)
{{
    return {opcode}value;
}}
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task WhileTest()
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

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task WhileNonCompoundTest()
    {
        var inputContents = @"int MyFunction(int count)
{
    int i = 0;

    while (i < count)
        i++;

    return i;
}
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }
}
