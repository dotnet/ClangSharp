// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

[TestFixtureSource(nameof(FixtureArgs))]
public sealed class EnumDeclarationTest(PInvokeGeneratorOutputMode outputMode, PInvokeGeneratorConfigurationOptions outputVersion)
    : PInvokeGeneratorTest(outputMode, outputVersion)
{
    private static readonly string[] s_excludeTestExcludedNames = ["MyEnum"];

    [Test]
    public Task BasicTest()
    {
        var inputContents = @"enum MyEnum : int
{
    MyEnum_Value0,
    MyEnum_Value1,
    MyEnum_Value2,
};
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task BasicValueTest()
    {
        var inputContents = @"enum MyEnum : int
{
    MyEnum_Value1 = 1,
    MyEnum_Value2,
    MyEnum_Value3,
};
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task ExcludeTest()
    {
        var inputContents = @"enum MyEnum : int
{
    MyEnum_Value0,
    MyEnum_Value1,
    MyEnum_Value2,
};
";

        return ValidateGeneratedBindingsAsync(inputContents, excludedNames: s_excludeTestExcludedNames);
    }

    [TestCase("short")]
    [TestCase("int")]
    [TestCase("long long")]
    [TestCase("unsigned")]
    public Task ExplicitTypedTest(string nativeType)
    {
        var inputContents = $@"enum MyEnum : {nativeType}
{{
    MyEnum_Value0,
    MyEnum_Value1,
    MyEnum_Value2,
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
    public Task ExplicitTypedWithNativeTypeNameTest(string nativeType)
    {
        var inputContents = $@"enum MyEnum : {nativeType}
{{
    MyEnum_Value0,
    MyEnum_Value1,
    MyEnum_Value2,
}};
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

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
        return ValidateGeneratedBindingsAsync(inputContents, remappedNames: remappedNames);
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
        return ValidateGeneratedBindingsAsync(inputContents, withAttributes: withAttributes);
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
        return ValidateGeneratedBindingsAsync(inputContents, withUsings: withNamespaces);
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
        return ValidateGeneratedBindingsAsync(inputContents, withUsings: withNamespaces);
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
        return ValidateGeneratedBindingsAsync(inputContents, withUsings: withNamespaces);
    }

    [Test]
    public Task WithCastToEnumType()
    {
        var inputContents = @"enum MyEnum : int
{
    MyEnum_Value0 = (MyEnum) 10,
    MyEnum_Value1 = (MyEnum) MyEnum_Value0,
    MyEnum_Value2 = ((MyEnum) 10) + MyEnum_Value1,
};
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task WithMultipleEnumsTest()
    {
        var inputContents = @"enum MyEnum1 : int
{
    MyEnum1_Value0 = 10,
};

enum MyEnum2 : int
{
    MyEnum2_Value0 = MyEnum1_Value0,
    MyEnum2_Value1 = MyEnum1_Value0 + (MyEnum1) 10,
};
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task WithicitConversionTest()
    {
        var inputContents = @"enum MyEnum : int
{
    MyEnum_Value0,
    MyEnum_Value1,
    MyEnum_Value2 = 0x80000000,
};
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

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
        return ValidateGeneratedBindingsAsync(inputContents, withTypes: withTypes);
    }

    [Test]
    public Task WithTypeAndicitConversionTest()
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
        return ValidateGeneratedBindingsAsync(inputContents, withTypes: withTypes);
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
        return ValidateGeneratedBindingsAsync(inputContents, withTypes: withTypes);
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
        return ValidateGeneratedBindingsAsync(inputContents, withTypes: withTypes);
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
        return ValidateGeneratedBindingsAsync(inputContents, expectedDiagnostics: diagnostics);
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
        return ValidateGeneratedBindingsAsync(inputContents, expectedDiagnostics: diagnostics);
    }
}
