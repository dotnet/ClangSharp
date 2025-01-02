// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

[TestFixtureSource(nameof(FixtureArgs))]
public sealed class DeprecatedToObsoleteTest(PInvokeGeneratorOutputMode outputMode, PInvokeGeneratorConfigurationOptions outputVersion)
    : PInvokeGeneratorTest(outputMode, outputVersion)
{
    [TestCase("int")]
    public Task SimpleStructMembers(string nativeType)
    {
        var inputContents = $@"struct MyStruct
{{
    {nativeType} r;
    
    [[deprecated]]
    {nativeType} g;

    [[deprecated(""This is obsolete."")]]
    {nativeType} b;

    {nativeType} a;
}};
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task StructDecl()
    {
        var inputContents = $@"struct MyStruct0
{{
    int r;
}};

struct [[deprecated]] MyStruct1
{{
    int r;
}};

struct [[deprecated(""This is obsolete."")]] MyStruct2
{{
    int r;
}};

struct MyStruct3
{{
    int r;
}};
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [TestCase("int")]
    public Task SimpleTypedefStructMembers(string nativeType)
    {
        var inputContents = $@"typedef struct
{{
    {nativeType} r;

    [[deprecated]]
    {nativeType} g;

    [[deprecated(""This is obsolete."")]]
    {nativeType} b;

    {nativeType} a;
}} MyStruct;
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task TypedefStructDecl()
    {
        var inputContents = $@"typedef struct
{{
    int r;
}} MyStruct0;

[[deprecated]] typedef struct
{{
    int r;
}} MyStruct1;

[[deprecated(""This is obsolete."")]] typedef struct
{{
    int r;
}} MyStruct2;

typedef struct
{{
    int r;
}} MyStruct3;
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task SimpleEnumMembers()
    {
        var inputContents = $@"enum MyEnum : int
{{
    MyEnum_Value0,
    MyEnum_Value1 [[deprecated]],
    MyEnum_Value2 [[deprecated(""This is obsolete."")]],
    MyEnum_Value3,
}};
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task EnumDecl()
    {
        var inputContents = $@"enum MyEnum0 : int
{{
    MyEnum_Value0,
}};

enum [[deprecated]] MyEnum1 : int
{{
    MyEnum_Value1,
}};

enum [[deprecated(""This is obsolete."")]] MyEnum2 : int
{{
    MyEnum_Value2,
}};


enum MyEnum3 : int
{{
    MyEnum_Value3,
}};
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [TestCase("int")]
    public Task SimpleVarDecl(string nativeType)
    {
        var inputContents = $@"
{nativeType} MyVariable0 = 0;

[[deprecated]]
{nativeType} MyVariable1 = 0;

[[deprecated(""This is obsolete."")]]
{nativeType} MyVariable2 = 0;

{nativeType} MyVariable3 = 0;";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task FuncDecl()
    {
        var inputContents = @"
void MyFunction0()
{
}

[[deprecated]]
void MyFunction1()
{
}

[[deprecated(""This is obsolete."")]]
void MyFunction2()
{
}

void MyFunction3()
{
}
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task InstanceFunc()
    {
        var inputContents = @"struct MyStruct
{
    int MyFunction0() { return 0; }

    [[deprecated]]
    int MyFunction1() { return 0; }

    [[deprecated(""This is obsolete."")]]
    int MyFunction2() { return 0; }

    int MyFunction3() { return 0; }
};";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task FuncPtrDecl()
    {
        var inputContents = @"
typedef void (*Callback0)();
[[deprecated]] typedef void (*Callback1)();
[[deprecated(""This is obsolete."")]] typedef void (*Callback2)();
typedef void (*Callback3)();

struct MyStruct0 {
    Callback0 _callback;
};
struct MyStruct1 {
    Callback1 _callback;
};
struct MyStruct2 {
    Callback2 _callback;
};
struct MyStruct3 {
    Callback3 _callback;
};
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task FuncDllImport()
    {
        var inputContents = @"
extern ""C"" void MyFunction0();
extern ""C"" [[deprecated]] void MyFunction1();
extern ""C"" [[deprecated(""This is obsolete."")]] void MyFunction2();
extern ""C"" void MyFunction3();";

        return ValidateGeneratedBindingsAsync(inputContents);
    }
}
