// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

[TestFixtureSource(nameof(FixtureArgs))]
public sealed class StructDeclarationTest(PInvokeGeneratorOutputMode outputMode, PInvokeGeneratorConfigurationOptions outputVersion)
    : PInvokeGeneratorTest(outputMode, outputVersion)
{
    private static readonly string[] s_excludeTestExcludedNames = ["MyStruct"];
    private static readonly string[] s_guidTestExcludedNames = ["DECLSPEC_UUID"];

    [TestCase("double")]
    [TestCase("short")]
    [TestCase("int")]
    [TestCase("float")]
    public Task IncompleteArraySizeTest(string nativeType)
    {
        var inputContents = $@"struct MyStruct
{{
    {nativeType} x[];
}};
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [TestCase("double")]
    [TestCase("short")]
    [TestCase("int")]
    [TestCase("float")]
    public Task BasicTest(string nativeType)
    {
        var inputContents = $@"struct MyStruct
{{
    {nativeType} r;
    {nativeType} g;
    {nativeType} b;
}};
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [TestCase("double")]
    [TestCase("short")]
    [TestCase("int")]
    [TestCase("float")]
    public Task BasicTestInCMode(string nativeType)
    {
        var inputContents = $@"typedef struct
{{
    {nativeType} r;
    {nativeType} g;
    {nativeType} b;
}}  MyStruct;
";

        return ValidateGeneratedBindingsAsync(inputContents, commandLineArgs: []);
    }

    [TestCase("unsigned char")]
    [TestCase("long long")]
    [TestCase("signed char")]
    [TestCase("unsigned short")]
    [TestCase("unsigned int")]
    [TestCase("unsigned long long")]
    [TestCase("bool")]
    public Task BasicWithNativeTypeNameTest(string nativeType)
    {
        var inputContents = $@"struct MyStruct
{{
    {nativeType} r;
    {nativeType} g;
    {nativeType} b;
}};
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task BitfieldTest()
    {
        var inputContents = @"struct MyStruct1
{
    unsigned int o0_b0_24 : 24;
    unsigned int o4_b0_16 : 16;
    unsigned int o4_b16_3 : 3;
    int o4_b19_3 : 3;
    unsigned char o4_b22_1 : 1;
    int o4_b23_1 : 1;
    int o4_b24_1 : 1;
};

struct MyStruct2
{
    unsigned int o0_b0_1 : 1;
    int x;
    unsigned int o8_b0_1 : 1;
};

struct MyStruct3
{
    unsigned int o0_b0_1 : 1;
    unsigned int o0_b1_1 : 1;
};
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task BitfieldWithNativeBitfieldAttributeTest()
    {
        var inputContents = @"struct MyStruct1
{
    unsigned int o0_b0_24 : 24;
    unsigned int o4_b0_16 : 16;
    unsigned int o4_b16_3 : 3;
    int o4_b19_3 : 3;
    unsigned char o4_b22_1 : 1;
    int o4_b23_1 : 1;
    int o4_b24_1 : 1;
};

struct MyStruct2
{
    unsigned int o0_b0_1 : 1;
    int x;
    unsigned int o8_b0_1 : 1;
};

struct MyStruct3
{
    unsigned int o0_b0_1 : 1;
    unsigned int o0_b1_1 : 1;
};
";

        return ValidateGeneratedBindingsAsync(inputContents, PInvokeGeneratorConfigurationOptions.GenerateNativeBitfieldAttribute);
    }

    [Test]
    public Task DeclTypeTest()
    {
        var inputContents = @"extern ""C"" void MyFunction();

typedef struct
{
    decltype(&MyFunction) _callback;
} MyStruct;
";
        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task ExcludeTest()
    {
        var inputContents = "typedef struct MyStruct MyStruct;";
        return ValidateGeneratedBindingsAsync(inputContents, excludedNames: s_excludeTestExcludedNames);
    }

    [TestCase("double")]
    [TestCase("short")]
    [TestCase("int")]
    [TestCase("float")]
    public Task FixedSizedBufferNonPrimitiveTest(string nativeType)
    {
        var inputContents = $@"struct MyStruct
{{
    {nativeType} value;
}};

struct MyOtherStruct
{{
    MyStruct c[3];
}};
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [TestCase("double")]
    [TestCase("short")]
    [TestCase("int")]
    [TestCase("float")]
    public Task FixedSizedBufferNonPrimitiveMultidimensionalTest(string nativeType)
    {
        var inputContents = $@"struct MyStruct
{{
    {nativeType} value;
}};

struct MyOtherStruct
{{
    MyStruct c[2][1][3][4];
}};
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [TestCase("double")]
    [TestCase("short")]
    [TestCase("int")]
    [TestCase("float")]
    public Task FixedSizedBufferNonPrimitiveTypedefTest(string nativeType)
    {
        var inputContents = $@"struct MyStruct
{{
    {nativeType} value;
}};

typedef MyStruct MyBuffer[3];

struct MyOtherStruct
{{
    MyBuffer c;
}};
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [TestCase("unsigned char")]
    [TestCase("long long")]
    [TestCase("signed char")]
    [TestCase("unsigned short")]
    [TestCase("unsigned int")]
    [TestCase("unsigned long long")]
    [TestCase("bool")]
    public Task FixedSizedBufferNonPrimitiveWithNativeTypeNameTest(string nativeType)
    {
        var inputContents = $@"struct MyStruct
{{
    {nativeType} value;
}};

struct MyOtherStruct
{{
    MyStruct c[3];
}};
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [TestCase("double *")]
    [TestCase("short *")]
    [TestCase("int *")]
    [TestCase("float *")]
    public Task FixedSizedBufferPointerTest(string nativeType)
    {
        var inputContents = $@"struct MyStruct
{{
    {nativeType} c[3];
}};
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [TestCase("unsigned char")]
    [TestCase("double")]
    [TestCase("short")]
    [TestCase("int")]
    [TestCase("long long")]
    [TestCase("signed char")]
    [TestCase("float")]
    [TestCase("unsigned short")]
    [TestCase("unsigned int")]
    [TestCase("unsigned long long")]
    public Task FixedSizedBufferPrimitiveTest(string nativeType)
    {
        var inputContents = $@"struct MyStruct
{{
    {nativeType} c[3];
}};
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [TestCase("unsigned char")]
    [TestCase("double")]
    [TestCase("short")]
    [TestCase("int")]
    [TestCase("long long")]
    [TestCase("signed char")]
    [TestCase("float")]
    [TestCase("unsigned short")]
    [TestCase("unsigned int")]
    [TestCase("unsigned long long")]
    public Task FixedSizedBufferPrimitiveMultidimensionalTest(string nativeType)
    {
        var inputContents = $@"struct MyStruct
{{
    {nativeType} c[2][1][3][4];
}};
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [TestCase("unsigned char")]
    [TestCase("double")]
    [TestCase("short")]
    [TestCase("int")]
    [TestCase("long long")]
    [TestCase("signed char")]
    [TestCase("float")]
    [TestCase("unsigned short")]
    [TestCase("unsigned int")]
    [TestCase("unsigned long long")]
    public Task FixedSizedBufferPrimitiveTypedefTest(string nativeType)
    {
        var inputContents = $@"typedef {nativeType} MyBuffer[3];

struct MyStruct
{{
    MyBuffer c;
}};
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task GuidTest()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // Non-Windows doesn't support __declspec(uuid(""))
            return Task.CompletedTask;
        }

        var inputContents = $@"#define DECLSPEC_UUID(x) __declspec(uuid(x))

struct __declspec(uuid(""00000000-0000-0000-C000-000000000046"")) MyStruct1
{{
    int x;
}};

struct DECLSPEC_UUID(""00000000-0000-0000-C000-000000000047"") MyStruct2
{{
    int x;
}};
";

        return ValidateGeneratedBindingsAsync(inputContents, excludedNames: s_guidTestExcludedNames);
    }

    [Test]
    public Task InheritanceTest()
    {
        var inputContents = @"struct MyStruct1A
{
    int x;
    int y;
};

struct MyStruct1B
{
    int x;
    int y;
};

struct MyStruct2 : MyStruct1A, MyStruct1B
{
    int z;
    int w;
};
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task InheritanceWithNativeInheritanceAttributeTest()
    {
        var inputContents = @"struct MyStruct1A
{
    int x;
    int y;
};

struct MyStruct1B
{
    int x;
    int y;
};

struct MyStruct2 : MyStruct1A, MyStruct1B
{
    int z;
    int w;
};
";

        return ValidateGeneratedBindingsAsync(inputContents, PInvokeGeneratorConfigurationOptions.GenerateNativeInheritanceAttribute);
    }

    [TestCase("double")]
    [TestCase("short")]
    [TestCase("int")]
    [TestCase("float")]
    public Task NestedAnonymousTest(string nativeType)
    {
        var inputContents = $@"typedef union {{
    {nativeType} value;
}} MyUnion;

struct MyStruct
{{
    {nativeType} x;
    {nativeType} y;

    struct
    {{
        {nativeType} z;

        struct
        {{
            {nativeType} value;
        }} w;

        struct
        {{
            {nativeType} value1;

            struct
            {{
                {nativeType} value;
            }};
        }};

        union
        {{
            {nativeType} value2;
        }};

        MyUnion u;
        {nativeType} buffer1[4];
        MyUnion buffer2[4];
    }};
}};
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task NestedAnonymousWithBitfieldTest()
    {
        var inputContents = @"struct MyStruct
{
    int x;
    int y;

    struct
    {
        int z;

        struct
        {
            int w;
            int o0_b0_16 : 16;
            int o0_b16_4 : 4;
        };
    };
};
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [TestCase("double")]
    [TestCase("short")]
    [TestCase("int")]
    [TestCase("float")]
    public Task NestedTest(string nativeType)
    {
        var inputContents = $@"struct MyStruct
{{
    {nativeType} r;
    {nativeType} g;
    {nativeType} b;

    struct MyNestedStruct
    {{
        {nativeType} r;
        {nativeType} g;
        {nativeType} b;
        {nativeType} a;
    }};
}};
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [TestCase("unsigned char")]
    [TestCase("long long")]
    [TestCase("signed char")]
    [TestCase("unsigned short")]
    [TestCase("unsigned int")]
    [TestCase("unsigned long long")]
    [TestCase("bool")]
    public Task NestedWithNativeTypeNameTest(string nativeType)
    {
        var inputContents = $@"struct MyStruct
{{
    {nativeType} r;
    {nativeType} g;
    {nativeType} b;

    struct MyNestedStruct
    {{
        {nativeType} r;
        {nativeType} g;
        {nativeType} b;
        {nativeType} a;
    }};
}};
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task NewKeywordTest()
    {
        var inputContents = @"struct MyStruct
{
    int Equals;
    int Dispose;
    int GetHashCode;
    int GetType;
    int MemberwiseClone;
    int ReferenceEquals;
    int ToString;
};";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task NoDefinitionTest()
    {
        var inputContents = "typedef struct MyStruct MyStruct;";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task PackTest()
    {
        const string InputContents = @"struct MyStruct1 {
    unsigned Field1;

    void* Field2;

    unsigned Field3;
};

#pragma pack(4)

struct MyStruct2 {
    unsigned Field1;

    void* Field2;

    unsigned Field3;
};
";

        return ValidateGeneratedBindingsAsync(InputContents);
    }

    [Test]
    public Task PointerToSelfTest()
    {
        var inputContents = @"struct example_s {
   example_s* next;
   void* data;
};";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task PointerToSelfViaTypedefTest()
    {
        var inputContents = @"typedef struct example_s example_t;

struct example_s {
   example_t* next;
   void* data;
};";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task RemapTest()
    {
        var inputContents = "typedef struct _MyStruct MyStruct;";

        var remappedNames = new Dictionary<string, string> { ["_MyStruct"] = "MyStruct" };
        return ValidateGeneratedBindingsAsync(inputContents, remappedNames: remappedNames);
    }

    [Test]
    public Task RemapNestedAnonymousTest()
    {
        var inputContents = @"struct MyStruct
{
    double r;
    double g;
    double b;

    struct
    {
        double a;
    };
};";

        var remappedNames = new Dictionary<string, string> {
            ["__AnonymousField_ClangUnsavedFile_L7_C5"] = "Anonymous",
            ["__AnonymousRecord_ClangUnsavedFile_L7_C5"] = "_Anonymous_e__Struct"
        };
        return ValidateGeneratedBindingsAsync(inputContents, remappedNames: remappedNames);
    }

    [TestCase("double")]
    [TestCase("short")]
    [TestCase("int")]
    [TestCase("float")]
    public Task SkipNonDefinitionTest(string nativeType)
    {
        var inputContents = $@"typedef struct MyStruct MyStruct;

struct MyStruct
{{
    {nativeType} r;
    {nativeType} g;
    {nativeType} b;
}};
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task SkipNonDefinitionPointerTest()
    {
        var inputContents = @"typedef struct MyStruct* MyStructPtr;
typedef struct MyStruct& MyStructRef;
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [TestCase("unsigned char")]
    [TestCase("long long")]
    [TestCase("signed char")]
    [TestCase("unsigned short")]
    [TestCase("unsigned int")]
    [TestCase("unsigned long long")]
    [TestCase("bool")]
    public Task SkipNonDefinitionWithNativeTypeNameTest(string nativeType)
    {
        var inputContents = $@"typedef struct MyStruct MyStruct;

struct MyStruct
{{
    {nativeType} r;
    {nativeType} g;
    {nativeType} b;
}};
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [TestCase("unsigned char")]
    [TestCase("double")]
    [TestCase("short")]
    [TestCase("int")]
    [TestCase("long long")]
    [TestCase("signed char")]
    [TestCase("float")]
    [TestCase("unsigned short")]
    [TestCase("unsigned int")]
    [TestCase("unsigned long long")]
    [TestCase("bool")]
    public Task TypedefTest(string nativeType)
    {
        var inputContents = $@"typedef {nativeType} MyTypedefAlias;

struct MyStruct
{{
    MyTypedefAlias r;
    MyTypedefAlias g;
    MyTypedefAlias b;
}};
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task UsingDeclarationTest()
    {
        var inputContents = @"struct MyStruct1A
{
    void MyMethod() { }
};

struct MyStruct1B : MyStruct1A
{
    using MyStruct1A::MyMethod;
};
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task WithAccessSpecifierTest()
    {
        var inputContents = @"struct MyStruct1
{
    int Field1;
    int Field2;
};

struct MyStruct2
{
    int Field1;
    int Field2;
};

struct MyStruct3
{
    int Field1;
    int Field2;
};
";

        var withAccessSpecifiers = new Dictionary<string, AccessSpecifier> {
            ["MyStruct1"] = AccessSpecifier.Private,
            ["MyStruct2"] = AccessSpecifier.Internal,
            ["Field1"] = AccessSpecifier.Private,
            ["MyStruct3.Field2"] = AccessSpecifier.Internal,
        };
        return ValidateGeneratedBindingsAsync(inputContents, withAccessSpecifiers: withAccessSpecifiers);
    }

    [Test]
    public Task WithPackingTest()
    {
        var sizeTDef = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ?
            string.Empty :
            "typedef int size_t;\n";

        var inputContents = @$"{sizeTDef}
struct MyStruct
{{
    size_t FixedBuffer[2];
}};
";

        var withPackings = new Dictionary<string, string> {
            ["MyStruct"] = "CustomPackValue"
        };
        return ValidateGeneratedBindingsAsync(inputContents,  withPackings: withPackings);
    }

    [Test]
    public Task SourceLocationAttributeTest()
    {
        const string InputContents = @"struct MyStruct
{
    int r;
    int g;
    int b;
};
";

        return ValidateGeneratedBindingsAsync(InputContents, PInvokeGeneratorConfigurationOptions.GenerateSourceLocationAttribute);
    }
}
