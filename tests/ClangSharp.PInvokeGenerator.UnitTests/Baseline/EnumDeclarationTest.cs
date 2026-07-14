// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests.Baseline;

[TestFixtureSource(nameof(Variants))]
public sealed class EnumDeclarationTest : BaselineTest
{
    private static readonly string[] ExcludeTestExcludedNames = ["MyEnum"];

    public EnumDeclarationTest(BaselineVariant variant) : base(variant)
    {
    }

    protected override string Area => "EnumDeclaration";

    [Test]
    public Task BasicTest()
        => ValidateAsync(nameof(BasicTest), @"enum MyEnum : int
{
    MyEnum_Value0,
    MyEnum_Value1,
    MyEnum_Value2,
};
");

    [Test]
    public Task BasicValueTest()
        => ValidateAsync(nameof(BasicValueTest), @"enum MyEnum : int
{
    MyEnum_Value1 = 1,
    MyEnum_Value2,
    MyEnum_Value3,
};
");

    [Test]
    public Task ExcludeTest()
        => ValidateAsync(nameof(ExcludeTest), @"enum MyEnum : int
{
    MyEnum_Value0,
    MyEnum_Value1,
    MyEnum_Value2,
};
", excludedNames: ExcludeTestExcludedNames);

    [TestCase("short", "short")]
    public Task ExplicitTypedTest(string nativeType, string expectedManagedType)
        => ValidateAsync(nameof(ExplicitTypedTest), $@"enum MyEnum : {nativeType}
{{
    MyEnum_Value0,
    MyEnum_Value1,
    MyEnum_Value2,
}};
", discriminator: $"{nativeType}_{expectedManagedType}");

    [TestCase("unsigned char", "byte")]
    [TestCase("long long", "long")]
    [TestCase("signed char", "sbyte")]
    [TestCase("unsigned short", "ushort")]
    [TestCase("unsigned int", "uint")]
    [TestCase("unsigned long long", "ulong")]
    public Task ExplicitTypedWithNativeTypeNameTest(string nativeType, string expectedManagedType)
        => ValidateAsync(nameof(ExplicitTypedWithNativeTypeNameTest), $@"enum MyEnum : {nativeType}
{{
    MyEnum_Value0,
    MyEnum_Value1,
    MyEnum_Value2,
}};
", discriminator: $"{nativeType}_{expectedManagedType}");

    [Test]
    public Task RemapTest()
    {
        var inputContents = @"typedef enum _MyEnum1 : int
{
    MyEnum1_Value1,
    MyEnum1_Value2,
    MyEnum1_Value3,
} MyEnum1;

namespace Namespace1
{
    namespace Namespace2
    {
        typedef enum _MyEnum2 : int
        {
            MyEnum2_Value1,
            MyEnum2_Value2,
            MyEnum2_Value3,
        } MyEnum2;

        typedef enum _MyEnum3 : int
        {
            MyEnum3_Value1,
            MyEnum3_Value2,
            MyEnum3_Value3,
        } MyEnum3;

        typedef enum _MyEnum4 : int
        {
            MyEnum4_Value1,
            MyEnum4_Value2,
            MyEnum4_Value3,
        } MyEnum4;
    }
}
";

        var remappedNames = new Dictionary<string, string> { ["_MyEnum1"] = "MyEnum1", ["Namespace1.Namespace2._MyEnum2"] = "MyEnum2", ["_MyEnum3"] = "MyEnum3", ["Namespace1::Namespace2::_MyEnum4"] = "MyEnum4" };
        return ValidateAsync(nameof(RemapTest), inputContents, remappedNames: remappedNames);
    }

    [Test]
    public Task WithAttributeTest()
    {
        var inputContents = @"enum MyEnum1 : int
{
    MyEnum1_Value1 = 1,
};

enum MyEnum2 : int
{
    MyEnum2_Value1 = 1,
};
";

        var withAttributes = new Dictionary<string, IReadOnlyList<string>>
        {
            ["MyEnum1"] = ["Flags"]
        };
        return ValidateAsync(nameof(WithAttributeTest), inputContents, withAttributes: withAttributes);
    }

    [Test]
    public Task WithNamespaceTest()
    {
        var inputContents = @"enum MyEnum1 : int
{
    MyEnum1_Value1 = 1,
};

enum MyEnum2 : int
{
    MyEnum2_Value1 = MyEnum1_Value1,
};
";

        var withNamespaces = new Dictionary<string, IReadOnlyList<string>>
        {
            ["MyEnum1"] = ["static ClangSharp.Test.MyEnum1"]
        };
        return ValidateAsync(nameof(WithNamespaceTest), inputContents, withUsings: withNamespaces);
    }

    [Test]
    public Task WithNamespaceStarTest()
    {
        var inputContents = @"enum MyEnum1 : int
{
    MyEnum1_Value1 = 1,
};

enum MyEnum2 : int
{
    MyEnum2_Value1 = MyEnum1_Value1,
};
";

        var withNamespaces = new Dictionary<string, IReadOnlyList<string>>
        {
            ["*"] = ["static ClangSharp.Test.MyEnum1"]
        };
        return ValidateAsync(nameof(WithNamespaceStarTest), inputContents, withUsings: withNamespaces);
    }

    [Test]
    public Task WithNamespaceStarPlusTest()
    {
        var inputContents = @"enum MyEnum1 : int
{
    MyEnum1_Value1 = 1,
};

enum MyEnum2 : int
{
    MyEnum2_Value1 = MyEnum1_Value1,
};
";

        var withNamespaces = new Dictionary<string, IReadOnlyList<string>>
        {
            ["*"] = ["static ClangSharp.Test.MyEnum1"],
            ["MyEnum2"] = ["System"]
        };
        return ValidateAsync(nameof(WithNamespaceStarPlusTest), inputContents, withUsings: withNamespaces);
    }

    [Test]
    public Task WithCastToEnumType()
        => ValidateAsync(nameof(WithCastToEnumType), @"enum MyEnum : int
{
    MyEnum_Value0 = (MyEnum) 10,
    MyEnum_Value1 = (MyEnum) MyEnum_Value0,
    MyEnum_Value2 = ((MyEnum) 10) + MyEnum_Value1,
};
");

    [Test]
    public Task WithMultipleEnumsTest()
        => ValidateAsync(nameof(WithMultipleEnumsTest), @"enum MyEnum1 : int
{
    MyEnum1_Value0 = 10,
};

enum MyEnum2 : int
{
    MyEnum2_Value0 = MyEnum1_Value0,
    MyEnum2_Value1 = MyEnum1_Value0 + (MyEnum1) 10,
};
");

    [Test]
    public Task WithImplicitConversionTest()
        => ValidateAsync(nameof(WithImplicitConversionTest), @"enum MyEnum : int
{
    MyEnum_Value0,
    MyEnum_Value1,
    MyEnum_Value2 = 0x80000000,
};
");

    [Test]
    public Task WithUnsignedInitConversionTest()
        => ValidateAsync(nameof(WithUnsignedInitConversionTest), @"enum MyEnum : int
{
    MyEnum_Value0,
    MyEnum_Value1 = (1U << 22),
    MyEnum_Value2 = (1U << 22) | (1 << 12),
    MyEnum_Value3 = 0x80000000,
};
");

    [Test]
    public Task WithTypeTest()
    {
        var inputContents = @"enum MyEnum : int
{
    MyEnum_Value0,
    MyEnum_Value1,
    MyEnum_Value2,
};
";

        var withTypes = new Dictionary<string, string> {
            ["MyEnum"] = "uint"
        };
        return ValidateAsync(nameof(WithTypeTest), inputContents, withTypes: withTypes);
    }

    [Test]
    public Task WithTypeAndImplicitConversionTest()
    {
        var inputContents = @"enum MyEnum : int
{
    MyEnum_Value0,
    MyEnum_Value1,
    MyEnum_Value2 = 0x80000000,
};
";

        var withTypes = new Dictionary<string, string>
        {
            ["MyEnum"] = "uint"
        };
        return ValidateAsync(nameof(WithTypeAndImplicitConversionTest), inputContents, withTypes: withTypes);
    }

    [Test]
    public Task WithTypeStarTest()
    {
        var inputContents = @"enum MyEnum1 : int
{
    MyEnum1_Value0
};

enum MyEnum2 : int
{
    MyEnum2_Value0
};
";

        var withTypes = new Dictionary<string, string>
        {
            ["*"] = "uint"
        };
        return ValidateAsync(nameof(WithTypeStarTest), inputContents, withTypes: withTypes);
    }

    [Test]
    public Task WithTypeStarOverrideTest()
    {
        var inputContents = @"enum MyEnum1 : int
{
    MyEnum1_Value0
};

enum MyEnum2 : int
{
    MyEnum2_Value0
};
";

        var withTypes = new Dictionary<string, string>
        {
            ["*"] = "uint",
            ["MyEnum1"] = "int",
        };
        return ValidateAsync(nameof(WithTypeStarOverrideTest), inputContents, withTypes: withTypes);
    }

    [Test]
    public Task WithAnonymousEnumTest()
    {
        var inputContents = @"enum
{
    MyEnum1_Value1 = 1,
};

enum MyEnum2 : int
{
    MyEnum2_Value1 = MyEnum1_Value1,
};
";

        var diagnostics = new[] { new Diagnostic(DiagnosticLevel.Info, "Found anonymous enum: __AnonymousEnum_ClangUnsavedFile_L1_C1. Mapping values as constants in: Methods", "Line 1, Column 1 in ClangUnsavedFile.h") };
        return ValidateAsync(nameof(WithAnonymousEnumTest), inputContents, expectedDiagnostics: diagnostics);
    }

    [Test]
    public Task WithReferenceToAnonymousEnumEnumeratorTest()
    {
        var inputContents = @"enum
{
    MyEnum1_Value1 = 1,
};

const int MyEnum2_Value1 = MyEnum1_Value1 + 1;
";
        var diagnostics = new[] { new Diagnostic(DiagnosticLevel.Info, "Found anonymous enum: __AnonymousEnum_ClangUnsavedFile_L1_C1. Mapping values as constants in: Methods", "Line 1, Column 1 in ClangUnsavedFile.h") };
        return ValidateAsync(nameof(WithReferenceToAnonymousEnumEnumeratorTest), inputContents, expectedDiagnostics: diagnostics);
    }
}
