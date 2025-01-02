// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

[TestFixtureSource(nameof(FixtureArgs))]
public sealed class UnionDeclarationTest(PInvokeGeneratorOutputMode outputMode, PInvokeGeneratorConfigurationOptions outputVersion)
    : PInvokeGeneratorTest(outputMode, outputVersion)
{
    private static readonly string[] s_excludeTestExcludedNames = ["MyUnion"];
    private static readonly string[] s_guidTestExcludedNames = ["DECLSPEC_UUID"];

    [TestCase("double")]
    [TestCase("short")]
    [TestCase("int")]
    [TestCase("float")]
    public Task BasicTest(string nativeType)
    {
        var inputContents = $@"union MyUnion
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
        var inputContents = $@"typedef union
{{
    {nativeType} r;
    {nativeType} g;
    {nativeType} b;
}}  MyUnion;
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
        var inputContents = $@"union MyUnion
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
        var inputContents = @"union MyUnion1
{
    unsigned int o0_b0_24 : 24;
    unsigned int o4_b0_16 : 16;
    unsigned int o4_b16_3 : 3;
    int o4_b19_3 : 3;
    unsigned char o4_b22_1 : 1;
    int o4_b23_1 : 1;
    int o4_b24_1 : 1;
};

union MyUnion2
{
    unsigned int o0_b0_1 : 1;
    int x;
    unsigned int o8_b0_1 : 1;
};

union MyUnion3
{
    unsigned int o0_b0_1 : 1;
    unsigned int o0_b1_1 : 1;
};
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task ExcludeTest()
    {
        var inputContents = "typedef union MyUnion MyUnion;";
        return ValidateGeneratedBindingsAsync(inputContents, excludedNames: s_excludeTestExcludedNames);
    }

    [TestCase("double")]
    [TestCase("short")]
    [TestCase("int")]
    [TestCase("float")]
    public Task FixedSizedBufferNonPrimitiveTest(string nativeType)
    {
        var inputContents = $@"union MyUnion
{{
    {nativeType} value;
}};

union MyOtherUnion
{{
    MyUnion c[3];
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
        var inputContents = $@"union MyUnion
{{
    {nativeType} value;
}};

union MyOtherUnion
{{
    MyUnion c[2][1][3][4];
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
        var inputContents = $@"union MyUnion
{{
    {nativeType} value;
}};

typedef MyUnion MyBuffer[3];

union MyOtherUnion
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
        var inputContents = $@"union MyUnion
{{
    {nativeType} value;
}};

union MyOtherUnion
{{
    MyUnion c[3];
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
        var inputContents = $@"union MyUnion
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
        var inputContents = $@"union MyUnion
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
        var inputContents = $@"union MyUnion
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

union MyUnion
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

union __declspec(uuid(""00000000-0000-0000-C000-000000000046"")) MyUnion1
{{
    int x;
}};

union DECLSPEC_UUID(""00000000-0000-0000-C000-000000000047"") MyUnion2
{{
    int x;
}};
";

        return ValidateGeneratedBindingsAsync(inputContents, excludedNames: s_guidTestExcludedNames);
    }

    [TestCase("double")]
    [TestCase("short")]
    [TestCase("int")]
    [TestCase("float")]
    public Task NestedAnonymousTest(string nativeType)
    {
        var inputContents = $@"typedef struct {{
    {nativeType} value;
}} MyStruct;

union MyUnion
{{
    {nativeType} r;
    {nativeType} g;
    {nativeType} b;

    union
    {{
        {nativeType} a;

        MyStruct s;

        {nativeType} buffer[4];
    }};
}};
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task NestedAnonymousWithBitfieldTest()
    {
        var inputContents = @"union MyUnion
{
    int x;
    int y;

    union
    {
        int z;

        union
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
        var inputContents = $@"union MyUnion
{{
    {nativeType} r;
    {nativeType} g;
    {nativeType} b;

    union MyNestedUnion
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
        var inputContents = $@"union MyUnion
{{
    {nativeType} r;
    {nativeType} g;
    {nativeType} b;

    union MyNestedUnion
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
        var inputContents = @"union MyUnion
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
        var inputContents = "typedef union MyUnion MyUnion;";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task PointerToSelfTest()
    {
        var inputContents = @"union example_s {
   example_s* next;
   void* data;
};";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task PointerToSelfViaTypedefTest()
    {
        var inputContents = @"typedef union example_s example_t;

union example_s {
   example_t* next;
   void* data;
};";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task RemapTest()
    {
        var inputContents = "typedef union _MyUnion MyUnion;";

        var remappedNames = new Dictionary<string, string> { ["_MyUnion"] = "MyUnion" };
        return ValidateGeneratedBindingsAsync(inputContents, remappedNames: remappedNames);
    }

    [Test]
    public Task RemapNestedAnonymousTest()
    {
        var inputContents = @"union MyUnion
{
    double r;
    double g;
    double b;

    union
    {
        double a;
    };
};";

        var remappedNames = new Dictionary<string, string> {
            ["__AnonymousField_ClangUnsavedFile_L7_C5"] = "Anonymous",
            ["__AnonymousRecord_ClangUnsavedFile_L7_C5"] = "_Anonymous_e__Union"
        };
        return ValidateGeneratedBindingsAsync(inputContents, remappedNames: remappedNames);
    }

    [TestCase("double")]
    [TestCase("short")]
    [TestCase("int")]
    [TestCase("float")]
    public Task SkipNonDefinitionTest(string nativeType)
    {
        var inputContents = $@"typedef union MyUnion MyUnion;

union MyUnion
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
        var inputContents = @"typedef union MyUnion* MyUnionPtr;
typedef union MyUnion& MyUnionRef;
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
        var inputContents = $@"typedef union MyUnion MyUnion;

union MyUnion
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

union MyUnion
{{
    MyTypedefAlias r;
    MyTypedefAlias g;
    MyTypedefAlias b;
}};
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task UnionWithAnonStructWithAnonUnion()
    {
        var inputContents = $@"typedef union _MY_UNION
{{
    long AsArray[2];
    struct
    {{
        long First;
        union
        {{
            struct
            {{
                long Second;
            }} A;

            struct
            {{
                long Second;
            }} B;
        }};
    }};
}} MY_UNION;";

        return ValidateGeneratedBindingsAsync(inputContents);
    }
}
