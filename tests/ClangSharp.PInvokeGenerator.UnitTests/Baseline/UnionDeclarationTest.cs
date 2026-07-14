// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests.Baseline;

[TestFixtureSource(nameof(Variants))]
public sealed class UnionDeclarationTest : BaselineTest
{
    private static readonly string[] ExcludeTestExcludedNames = ["MyUnion"];
    private static readonly string[] GuidTestExcludedNames = ["DECLSPEC_UUID"];

    public UnionDeclarationTest(BaselineVariant variant) : base(variant)
    {
    }

    protected override string Area => "UnionDeclaration";

    [TestCase("double", "double")]
    [TestCase("short", "short")]
    [TestCase("int", "int")]
    [TestCase("float", "float")]
    public Task BasicTest(string nativeType, string expectedManagedType)
    {
        var inputContents = $@"union MyUnion
{{
    {nativeType} r;
    {nativeType} g;
    {nativeType} b;
}};
";

        return ValidateAsync(nameof(BasicTest), inputContents, discriminator: $"{nativeType}_{expectedManagedType}");
    }

    [TestCase("double", "double")]
    [TestCase("short", "short")]
    [TestCase("int", "int")]
    [TestCase("float", "float")]
    public Task BasicTestInCMode(string nativeType, string expectedManagedType)
    {
        var inputContents = $@"typedef union
{{
    {nativeType} r;
    {nativeType} g;
    {nativeType} b;
}}  MyUnion;
";

        return ValidateAsync(nameof(BasicTestInCMode), inputContents, discriminator: $"{nativeType}_{expectedManagedType}", commandLineArgs: []);
    }

    [TestCase("unsigned char", "byte")]
    [TestCase("long long", "long")]
    [TestCase("signed char", "sbyte")]
    [TestCase("unsigned short", "ushort")]
    [TestCase("unsigned int", "uint")]
    [TestCase("unsigned long long", "ulong")]
    [TestCase("bool", "byte")]
    public Task BasicWithNativeTypeNameTest(string nativeType, string expectedManagedType)
    {
        var inputContents = $@"union MyUnion
{{
    {nativeType} r;
    {nativeType} g;
    {nativeType} b;
}};
";

        return ValidateAsync(nameof(BasicWithNativeTypeNameTest), inputContents, discriminator: $"{nativeType}_{expectedManagedType}");
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
    unsigned char o8_b0_1 : 1;
    int o12_b0_1 : 1;
    int o12_b1_1 : 1;
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

        return ValidateAsync(nameof(BitfieldTest), inputContents);
    }

    [Test]
    public Task ExcludeTest()
    {
        var inputContents = "typedef union MyUnion MyUnion;";
        return ValidateAsync(nameof(ExcludeTest), inputContents, excludedNames: ExcludeTestExcludedNames);
    }

    [TestCase("double", "double")]
    [TestCase("short", "short")]
    [TestCase("int", "int")]
    [TestCase("float", "float")]
    public Task FixedSizedBufferNonPrimitiveTest(string nativeType, string expectedManagedType)
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

        return ValidateAsync(nameof(FixedSizedBufferNonPrimitiveTest), inputContents, discriminator: $"{nativeType}_{expectedManagedType}");
    }

    [TestCase("double", "double")]
    [TestCase("short", "short")]
    [TestCase("int", "int")]
    [TestCase("float", "float")]
    public Task FixedSizedBufferNonPrimitiveMultidimensionalTest(string nativeType, string expectedManagedType)
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

        return ValidateAsync(nameof(FixedSizedBufferNonPrimitiveMultidimensionalTest), inputContents, discriminator: $"{nativeType}_{expectedManagedType}");
    }

    [TestCase("double", "double")]
    [TestCase("short", "short")]
    [TestCase("int", "int")]
    [TestCase("float", "float")]
    public Task FixedSizedBufferNonPrimitiveTypedefTest(string nativeType, string expectedManagedType)
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

        return ValidateAsync(nameof(FixedSizedBufferNonPrimitiveTypedefTest), inputContents, discriminator: $"{nativeType}_{expectedManagedType}");
    }

    [TestCase("unsigned char", "byte")]
    [TestCase("long long", "long")]
    [TestCase("signed char", "sbyte")]
    [TestCase("unsigned short", "ushort")]
    [TestCase("unsigned int", "uint")]
    [TestCase("unsigned long long", "ulong")]
    [TestCase("bool", "byte")]
    public Task FixedSizedBufferNonPrimitiveWithNativeTypeNameTest(string nativeType, string expectedManagedType)
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

        return ValidateAsync(nameof(FixedSizedBufferNonPrimitiveWithNativeTypeNameTest), inputContents, discriminator: $"{nativeType}_{expectedManagedType}");
    }

    [TestCase("double *", "double*")]
    [TestCase("short *", "short*")]
    [TestCase("int *", "int*")]
    [TestCase("float *", "float*")]
    public Task FixedSizedBufferPointerTest(string nativeType, string expectedManagedType)
    {
        var inputContents = $@"union MyUnion
{{
    {nativeType} c[3];
}};
";

        return ValidateAsync(nameof(FixedSizedBufferPointerTest), inputContents, discriminator: $"{nativeType}_{expectedManagedType}");
    }

    [TestCase("unsigned char", "byte")]
    [TestCase("double", "double")]
    [TestCase("short", "short")]
    [TestCase("int", "int")]
    [TestCase("long long", "long")]
    [TestCase("signed char", "sbyte")]
    [TestCase("float", "float")]
    [TestCase("unsigned short", "ushort")]
    [TestCase("unsigned int", "uint")]
    [TestCase("unsigned long long", "ulong")]
    public Task FixedSizedBufferPrimitiveTest(string nativeType, string expectedManagedType)
    {
        var inputContents = $@"union MyUnion
{{
    {nativeType} c[3];
}};
";

        return ValidateAsync(nameof(FixedSizedBufferPrimitiveTest), inputContents, discriminator: $"{nativeType}_{expectedManagedType}");
    }

    [TestCase("unsigned char", "byte")]
    [TestCase("double", "double")]
    [TestCase("short", "short")]
    [TestCase("int", "int")]
    [TestCase("long long", "long")]
    [TestCase("signed char", "sbyte")]
    [TestCase("float", "float")]
    [TestCase("unsigned short", "ushort")]
    [TestCase("unsigned int", "uint")]
    [TestCase("unsigned long long", "ulong")]
    public Task FixedSizedBufferPrimitiveMultidimensionalTest(string nativeType, string expectedManagedType)
    {
        var inputContents = $@"union MyUnion
{{
    {nativeType} c[2][1][3][4];
}};
";

        return ValidateAsync(nameof(FixedSizedBufferPrimitiveMultidimensionalTest), inputContents, discriminator: $"{nativeType}_{expectedManagedType}");
    }

    [TestCase("unsigned char", "byte")]
    [TestCase("double", "double")]
    [TestCase("short", "short")]
    [TestCase("int", "int")]
    [TestCase("long long", "long")]
    [TestCase("signed char", "sbyte")]
    [TestCase("float", "float")]
    [TestCase("unsigned short", "ushort")]
    [TestCase("unsigned int", "uint")]
    [TestCase("unsigned long long", "ulong")]
    public Task FixedSizedBufferPrimitiveTypedefTest(string nativeType, string expectedManagedType)
    {
        var inputContents = $@"typedef {nativeType} MyBuffer[3];

union MyUnion
{{
    MyBuffer c;
}};
";

        return ValidateAsync(nameof(FixedSizedBufferPrimitiveTypedefTest), inputContents, discriminator: $"{nativeType}_{expectedManagedType}");
    }

    [Test]
    public Task GuidTest()
    {
        if (Variant.Os != BaselineOs.Windows)
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

        return ValidateAsync(nameof(GuidTest), inputContents, excludedNames: GuidTestExcludedNames);
    }

    [TestCase("double", "double", 11, 5)]
    [TestCase("short", "short", 11, 5)]
    [TestCase("int", "int", 11, 5)]
    [TestCase("float", "float", 11, 5)]
    public Task NestedAnonymousTest(string nativeType, string expectedManagedType, int line, int column)
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

        return ValidateAsync(nameof(NestedAnonymousTest), inputContents, discriminator: $"{nativeType}_{expectedManagedType}_{line}_{column}");
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

        return ValidateAsync(nameof(NestedAnonymousWithBitfieldTest), inputContents);
    }

    [TestCase("double", "double")]
    [TestCase("short", "short")]
    [TestCase("int", "int")]
    [TestCase("float", "float")]
    public Task NestedTest(string nativeType, string expectedManagedType)
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

        return ValidateAsync(nameof(NestedTest), inputContents, discriminator: $"{nativeType}_{expectedManagedType}");
    }

    [TestCase("unsigned char", "byte")]
    [TestCase("long long", "long")]
    [TestCase("signed char", "sbyte")]
    [TestCase("unsigned short", "ushort")]
    [TestCase("unsigned int", "uint")]
    [TestCase("unsigned long long", "ulong")]
    [TestCase("bool", "byte")]
    public Task NestedWithNativeTypeNameTest(string nativeType, string expectedManagedType)
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

        return ValidateAsync(nameof(NestedWithNativeTypeNameTest), inputContents, discriminator: $"{nativeType}_{expectedManagedType}");
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

        return ValidateAsync(nameof(NewKeywordTest), inputContents);
    }

    [Test]
    public Task NoDefinitionTest()
    {
        var inputContents = "typedef union MyUnion MyUnion;";
        return ValidateAsync(nameof(NoDefinitionTest), inputContents);
    }

    [Test]
    public Task PointerToSelfTest()
    {
        var inputContents = @"union example_s {
   example_s* next;
   void* data;
};";

        return ValidateAsync(nameof(PointerToSelfTest), inputContents);
    }

    [Test]
    public Task PointerToSelfViaTypedefTest()
    {
        var inputContents = @"typedef union example_s example_t;

union example_s {
   example_t* next;
   void* data;
};";

        return ValidateAsync(nameof(PointerToSelfViaTypedefTest), inputContents);
    }

    [Test]
    public Task RemapTest()
    {
        var inputContents = "typedef union _MyUnion MyUnion;";

        var remappedNames = new Dictionary<string, string> { ["_MyUnion"] = "MyUnion" };
        return ValidateAsync(nameof(RemapTest), inputContents, remappedNames: remappedNames);
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
        return ValidateAsync(nameof(RemapNestedAnonymousTest), inputContents, remappedNames: remappedNames);
    }

    [TestCase("double", "double")]
    [TestCase("short", "short")]
    [TestCase("int", "int")]
    [TestCase("float", "float")]
    public Task SkipNonDefinitionTest(string nativeType, string expectedManagedType)
    {
        var inputContents = $@"typedef union MyUnion MyUnion;

union MyUnion
{{
    {nativeType} r;
    {nativeType} g;
    {nativeType} b;
}};
";

        return ValidateAsync(nameof(SkipNonDefinitionTest), inputContents, discriminator: $"{nativeType}_{expectedManagedType}");
    }

    [Test]
    public Task SkipNonDefinitionPointerTest()
    {
        var inputContents = @"typedef union MyUnion* MyUnionPtr;
typedef union MyUnion& MyUnionRef;
";

        return ValidateAsync(nameof(SkipNonDefinitionPointerTest), inputContents);
    }

    [TestCase("unsigned char", "byte")]
    [TestCase("long long", "long")]
    [TestCase("signed char", "sbyte")]
    [TestCase("unsigned short", "ushort")]
    [TestCase("unsigned int", "uint")]
    [TestCase("unsigned long long", "ulong")]
    [TestCase("bool", "byte")]
    public Task SkipNonDefinitionWithNativeTypeNameTest(string nativeType, string expectedManagedType)
    {
        var inputContents = $@"typedef union MyUnion MyUnion;

union MyUnion
{{
    {nativeType} r;
    {nativeType} g;
    {nativeType} b;
}};
";

        return ValidateAsync(nameof(SkipNonDefinitionWithNativeTypeNameTest), inputContents, discriminator: $"{nativeType}_{expectedManagedType}");
    }

    [TestCase("unsigned char", "byte")]
    [TestCase("double", "double")]
    [TestCase("short", "short")]
    [TestCase("int", "int")]
    [TestCase("long long", "long")]
    [TestCase("signed char", "sbyte")]
    [TestCase("float", "float")]
    [TestCase("unsigned short", "ushort")]
    [TestCase("unsigned int", "uint")]
    [TestCase("unsigned long long", "ulong")]
    [TestCase("bool", "byte")]
    public Task TypedefTest(string nativeType, string expectedManagedType)
    {
        var inputContents = $@"typedef {nativeType} MyTypedefAlias;

union MyUnion
{{
    MyTypedefAlias r;
    MyTypedefAlias g;
    MyTypedefAlias b;
}};
";

        return ValidateAsync(nameof(TypedefTest), inputContents, discriminator: $"{nativeType}_{expectedManagedType}");
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

        return ValidateAsync(nameof(UnionWithAnonStructWithAnonUnion), inputContents);
    }
}
