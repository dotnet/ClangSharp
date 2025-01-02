// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Runtime.InteropServices;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

[TestFixtureSource(nameof(FixtureArgs))]
public sealed class CXXMethodDeclarationTest(PInvokeGeneratorOutputMode outputMode, PInvokeGeneratorConfigurationOptions outputVersion)
    : PInvokeGeneratorTest(outputMode, outputVersion)
{
    [Test]
    public Task ConstructorTest()
    {
        var inputContents = @"struct MyStruct
{
    int _value;

    MyStruct(int value)
    {
        _value = value;
    }
};
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task ConstructorWithInitializeTest()
    {
        var inputContents = @"struct MyStruct
{
    int _x;
    int _y;
    int _z;

    MyStruct(int x) : _x(x)
    {
    }

    MyStruct(int x, int y) : _x(x), _y(y)
    {
    }

    MyStruct(int x, int y, int z) : _x(x), _y(y), _z()
    {
    }
};
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task ConversionTest()
    {
        var inputContents = @"struct MyStruct
{
    int value;

    operator int()
    {
        return value;
    }
};
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task DefaultParameterInheritedFromTemplateTest()
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

        return ValidateGeneratedBindingsAsync(InputContents, osxTransform: (output) => output.Replace("__ZN10MyTemplateIiE6DoWorkEPi", "_ZN10MyTemplateIiE6DoWorkEPi", System.StringComparison.Ordinal));
    }

    [Test]
    public Task DestructorTest()
    {
        var inputContents = @"struct MyStruct
{
    ~MyStruct()
    {
    }
};
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task InstanceTest()
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

        return ValidateGeneratedBindingsAsync(inputContents, osxTransform: (output) => output.Replace("__ZN8MyStruct12MyVoidMethodEv", "_ZN8MyStruct12MyVoidMethodEv", System.StringComparison.Ordinal));
    }

    [Test]
    public Task MemberCallTest()
    {
        var inputContents = @"struct MyStruct
{
    int value;

    int MyFunction1()
    {
        return value;
    }

    int MyFunction2()
    {
        return MyFunction1();
    }

    int MyFunction3()
    {
        return this->MyFunction1();
    }
};

int MyFunctionA(MyStruct x)
{
    return x.MyFunction1();
}

int MyFunctionB(MyStruct* x)
{
    return x->MyFunction2();
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

    int MyFunction()
    {
        return value;
    }
};
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task NewKeywordTest()
    {
        var inputContents = @"struct MyStruct
{
    int Equals() { return 0; }
    int Equals(int obj) { return 0; }
    int Dispose() { return 0; }
    int Dispose(int obj) { return 0; }
    int GetHashCode() { return 0; }
    int GetHashCode(int obj) { return 0; }
    int GetType() { return 0; }
    int GetType(int obj) { return 0; }
    int MemberwiseClone() { return 0; }
    int MemberwiseClone(int obj) { return 0; }
    int ReferenceEquals() { return 0; }
    int ReferenceEquals(int obj) { return 0; }
    int ToString() { return 0; }
    int ToString(int obj) { return 0; }
};";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task NewKeywordVirtualTest()
    {
        var inputContents = @"struct MyStruct
{
    virtual int GetType(int obj) = 0;
    virtual int GetType() = 0;
    virtual int GetType(int objA, int objB) = 0;
};";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task NewKeywordVirtualWithExplicitVtblTest()
    {
        var inputContents = @"struct MyStruct
{
    virtual int GetType(int obj) = 0;
    virtual int GetType() = 0;
    virtual int GetType(int objA, int objB) = 0;
};";

        return ValidateGeneratedBindingsAsync(inputContents, PInvokeGeneratorConfigurationOptions.GenerateExplicitVtbls);
    }

    [Test]
    public Task NewKeywordVirtualWithExplicitVtblAndMarkerInterfaceTest()
    {
        var inputContents = @"struct MyStruct
{
    virtual int GetType(int obj) = 0;
    virtual int GetType() = 0;
    virtual int GetType(int objA, int objB) = 0;
};";

        return ValidateGeneratedBindingsAsync(inputContents, PInvokeGeneratorConfigurationOptions.GenerateExplicitVtbls | PInvokeGeneratorConfigurationOptions.GenerateMarkerInterfaces);
    }

    [Test]
    public Task OperatorTest()
    {
        var inputContents = @"struct MyStruct
{
    int value;

    MyStruct(int value) : value(value)
    {
    }

    MyStruct operator+(MyStruct rhs)
    {
        return MyStruct(value + rhs.value);
    }
};

MyStruct operator-(MyStruct lhs, MyStruct rhs)
{
    return MyStruct(lhs.value - rhs.value);
}
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task OperatorCallTest()
    {
        var inputContents = @"struct MyStruct
{
    int value;

    MyStruct(int value) : value(value)
    {
    }

    MyStruct operator+(MyStruct rhs)
    {
        return MyStruct(value + rhs.value);
    }
};

MyStruct MyFunction1(MyStruct lhs, MyStruct rhs)
{
    return lhs + rhs;
}

MyStruct operator-(MyStruct lhs, MyStruct rhs)
{
    return MyStruct(lhs.value - rhs.value);
}

MyStruct MyFunction2(MyStruct lhs, MyStruct rhs)
{
    return lhs - rhs;
}
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task StaticTest()
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

        return ValidateGeneratedBindingsAsync(inputContents, osxTransform: (output) => output.Replace("__ZN8MyStruct12MyVoidMethodEv", "_ZN8MyStruct12MyVoidMethodEv", System.StringComparison.Ordinal));
    }

    [Test]
    public Task ThisTest()
    {
        var inputContents = @"struct MyStruct
{
    int value;

    int MyFunction()
    {
        return this->value;
    }
};
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task UnsafeDoesNotImpactDllImportTest()
    {
        var inputContents = @"struct MyStruct
{
    void* MyVoidStarMethod()
    {
        return nullptr;
    }
};

extern ""C"" void MyFunction();";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task VirtualTest()
    {
        var inputContents = @"struct MyStruct
{
    virtual void MyVoidMethod() = 0;

    virtual char MyInt8Method()
    {
        return 0;
    }

    virtual int MyInt32Method();

    virtual void* MyVoidStarMethod() = 0;
};
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task VirtualWithVtblIndexAttributeTest()
    {
        var inputContents = @"struct MyStruct
{
    virtual void MyVoidMethod() = 0;

    virtual char MyInt8Method()
    {
        return 0;
    }

    virtual int MyInt32Method();

    virtual void* MyVoidStarMethod() = 0;
};
";

        return ValidateGeneratedBindingsAsync(inputContents, PInvokeGeneratorConfigurationOptions.GenerateVtblIndexAttribute);
    }

    [Test]
    public Task MacrosExpansionTest()
    {
        var inputContents = @"typedef struct
{
	unsigned char *buf;
	int size;
} context_t;

int buf_close(void *pContext)
{
	((context_t*)pContext)->buf=0;
	return 0;
}
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }
}
