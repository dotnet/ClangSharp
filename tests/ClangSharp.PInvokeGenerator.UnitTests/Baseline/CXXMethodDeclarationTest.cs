// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests.Baseline;

[TestFixtureSource(nameof(Variants))]
public sealed class CXXMethodDeclarationTest : BaselineTest
{
    public CXXMethodDeclarationTest(BaselineVariant variant) : base(variant)
    {
    }

    protected override string Area => "CXXMethodDeclaration";

    [Test]
    public Task ConstructorTest()
        => ValidateAsync(nameof(ConstructorTest), @"struct MyStruct
{
    int _value;

    MyStruct(int value)
    {
        _value = value;
    }
};
");

    [Test]
    public Task ConstructorWithInitializeTest()
        => ValidateAsync(nameof(ConstructorWithInitializeTest), @"struct MyStruct
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
");

    [Test]
    public Task ConversionTest()
        => ValidateAsync(nameof(ConversionTest), @"struct MyStruct
{
    int value;
    int* pointer;
    int** pointer2;
    int*** pointer3;
    operator int()
    {
        return value;
    }
    operator int*()
    {
        return pointer;
    }
    operator int**()
    {
        return pointer2;
    }
    operator int***()
    {
        return pointer3;
    }
};
");

    [Test]
    public Task DefaultParameterInheritedFromTemplateTest()
        => ValidateAsync(nameof(DefaultParameterInheritedFromTemplateTest), @"template <typename T>
struct MyTemplate
{
    int* DoWork(int* value = nullptr)
    {
        return value;
    }
};

struct MyStruct : public MyTemplate<int>
{};
");

    [Test]
    public Task DestructorTest()
        => ValidateAsync(nameof(DestructorTest), @"struct MyStruct
{
    ~MyStruct()
    {
    }
};
");

    [Test]
    public Task OutOfLineTemplateConstructorDefinitionTest()
        => ValidateAsync(nameof(OutOfLineTemplateConstructorDefinitionTest), @"template <class T, class P>
struct MyCallResult
{
    MyCallResult();
    int _value;
};

template <class T, class P>
inline MyCallResult<T, P>::MyCallResult()
{
    _value = 0;
}
");

    [Test]
    public Task InstanceTest()
        => ValidateAsync(nameof(InstanceTest), @"struct MyStruct
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
");

    [Test]
    public Task MacrosExpansionTest()
        => ValidateAsync(nameof(MacrosExpansionTest), @"typedef struct
{
    unsigned char *buf;
    int size;
} context_t;

int buf_close(void *pContext)
{
    ((context_t*)pContext)->buf=0;
    return 0;
}
");

    [Test]
    public Task MemberCallTest()
        => ValidateAsync(nameof(MemberCallTest), @"struct MyStruct
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
");

    [Test]
    public Task MemberTest()
        => ValidateAsync(nameof(MemberTest), @"struct MyStruct
{
    int value;

    int MyFunction()
    {
        return value;
    }
};
");

    [Test]
    public Task NewKeywordTest()
        => ValidateAsync(nameof(NewKeywordTest), @"struct MyStruct
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
};");

    [Test]
    public Task NewKeywordVirtualTest()
        => ValidateAsync(nameof(NewKeywordVirtualTest), @"struct MyStruct
{
    virtual int GetType(int obj) = 0;
    virtual int GetType() = 0;
    virtual int GetType(int objA, int objB) = 0;
};");

    [Test]
    public Task NewKeywordVirtualWithExplicitVtblTest()
        => ValidateAsync(nameof(NewKeywordVirtualWithExplicitVtblTest), @"struct MyStruct
{
    virtual int GetType(int obj) = 0;
    virtual int GetType() = 0;
    virtual int GetType(int objA, int objB) = 0;
};", additionalConfigOptions: PInvokeGeneratorConfigurationOptions.GenerateExplicitVtbls);

    [Test]
    public Task NewKeywordVirtualWithExplicitVtblAndMarkerInterfaceTest()
        => ValidateAsync(nameof(NewKeywordVirtualWithExplicitVtblAndMarkerInterfaceTest), @"struct MyStruct
{
    virtual int GetType(int obj) = 0;
    virtual int GetType() = 0;
    virtual int GetType(int objA, int objB) = 0;
};", additionalConfigOptions: PInvokeGeneratorConfigurationOptions.GenerateExplicitVtbls | PInvokeGeneratorConfigurationOptions.GenerateMarkerInterfaces);

    [Test]
    public Task OperatorTest()
        => ValidateAsync(nameof(OperatorTest), @"struct MyStruct
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
");

    [Test]
    public Task OperatorCallTest()
        => ValidateAsync(nameof(OperatorCallTest), @"struct MyStruct
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
");

    [Test]
    public Task CompoundAssignmentOperatorTest()
        => ValidateAsync(nameof(CompoundAssignmentOperatorTest), @"struct MyStruct
{
    int value;

    MyStruct& operator+=(MyStruct rhs)
    {
        value += rhs.value;
        return *this;
    }

    MyStruct& operator*=(int scale)
    {
        value *= scale;
        return *this;
    }

    MyStruct& operator/=(MyStruct& other)
    {
        value /= other.value;
        return other;
    }
};
");

    [Test]
    public Task StaticTest()
        => ValidateAsync(nameof(StaticTest), @"struct MyStruct
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
");

    [Test]
    public Task ThisTest()
        => ValidateAsync(nameof(ThisTest), @"struct MyStruct
{
    int value;

    int MyFunction()
    {
        return this->value;
    }
};
");

    [Test]
    public Task UnsafeDoesNotImpactDllImportTest()
        => ValidateAsync(nameof(UnsafeDoesNotImpactDllImportTest), @"struct MyStruct
{
    void* MyVoidStarMethod()
    {
        return nullptr;
    }
};

extern ""C"" void MyFunction();");

    [Test]
    public Task VirtualTest()
        => ValidateAsync(nameof(VirtualTest), @"struct MyStruct
{
    virtual void MyVoidMethod() = 0;

    virtual signed char MyInt8Method()
    {
        return 0;
    }

    virtual int MyInt32Method();

    virtual void* MyVoidStarMethod() = 0;
};
");

    [Test]
    public Task VirtualWithVtblIndexAttributeTest()
        => ValidateAsync(nameof(VirtualWithVtblIndexAttributeTest), @"struct MyStruct
{
    virtual void MyVoidMethod() = 0;

    virtual signed char MyInt8Method()
    {
        return 0;
    }

    virtual int MyInt32Method();

    virtual void* MyVoidStarMethod() = 0;
};
", additionalConfigOptions: PInvokeGeneratorConfigurationOptions.GenerateVtblIndexAttribute);
}
