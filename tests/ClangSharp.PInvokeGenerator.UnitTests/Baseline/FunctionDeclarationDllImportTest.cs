// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests.Baseline;

[TestFixtureSource(nameof(Variants))]
public sealed class FunctionDeclarationDllImportTest : BaselineTest
{
    private static readonly string[] TemplateTestExcludedNames = ["MyTemplate"];

    public FunctionDeclarationDllImportTest(BaselineVariant variant) : base(variant)
    {
    }

    protected override string Area => "FunctionDeclarationDllImport";

    [Test]
    public Task BasicTest()
    {
        var inputContents = @"extern ""C"" void MyFunction();";

        return ValidateAsync(nameof(BasicTest), inputContents);
    }

    [Test]
    public Task ArrayParameterTest()
    {
        var inputContents = @"extern ""C"" void MyFunction(const float color[4]);";

        return ValidateAsync(nameof(ArrayParameterTest), inputContents);
    }

    [Test]
    public Task FunctionPointerParameterTest()
    {
        var inputContents = @"extern ""C"" void MyFunction(void (*callback)());";

        return ValidateAsync(nameof(FunctionPointerParameterTest), inputContents);
    }

    [Test]
    public Task NamespaceTest()
    {
        var inputContents = @"namespace MyNamespace
{
    void MyFunction();
}";

        return ValidateAsync(nameof(NamespaceTest), inputContents);
    }

    [TestCase("int", false, "int", "")]
    [TestCase("bool", true, "byte", "")]
    [TestCase("float *", true, "IntPtr", "using System;\n")]
    [TestCase("void (*)(int)", true, "IntPtr", "using System;\n")]
    public Task TemplateParameterTest(string nativeType, bool expectedNativeTypeAttr, string expectedManagedType, string expectedUsingStatement)
    {
        var inputContents = @$"template <typename T> struct MyTemplate;

extern ""C"" void MyFunction(MyTemplate<{nativeType}> myStruct);";

        return ValidateAsync(nameof(TemplateParameterTest), inputContents, discriminator: $"{nativeType}_{expectedNativeTypeAttr}_{expectedManagedType}_{expectedUsingStatement}", excludedNames: TemplateTestExcludedNames);
    }

    [Test]
    public Task TemplateMemberTest()
    {
        var inputContents = @$"template <typename T> struct MyTemplate
{{
}};

struct MyStruct
{{
    MyTemplate<float*> a;
}};
";

        return ValidateAsync(nameof(TemplateMemberTest), inputContents, excludedNames: TemplateTestExcludedNames);
    }

    [Test]
    public Task NoLibraryPathTest()
    {
        var inputContents = @"extern ""C"" void MyFunction();";

        return ValidateAsync(nameof(NoLibraryPathTest), inputContents, libraryPath: string.Empty);
    }

    [Test]
    public Task WithLibraryPathTest()
    {
        var inputContents = @"extern ""C"" void MyFunction();";

        var withLibraryPaths = new Dictionary<string, string>
        {
            ["MyFunction"] = "ClangSharpPInvokeGenerator"
        };
        return ValidateAsync(nameof(WithLibraryPathTest), inputContents, libraryPath: string.Empty, withLibraryPaths: withLibraryPaths);
    }

    [Test]
    public Task WithLibraryPathStarTest()
    {
        var inputContents = @"extern ""C"" void MyFunction();";

        var withLibraryPaths = new Dictionary<string, string>
        {
            ["*"] = "ClangSharpPInvokeGenerator"
        };
        return ValidateAsync(nameof(WithLibraryPathStarTest), inputContents, libraryPath: string.Empty, withLibraryPaths: withLibraryPaths);
    }

    [TestCase("unsigned char", "0", true, "byte", "0")]
    [TestCase("double", "1.0", false, "double", "1.0")]
    [TestCase("short", "2", false, "short", "2")]
    [TestCase("int", "3", false, "int", "3")]
    [TestCase("long long", "4", true, "long", "4")]
    [TestCase("signed char", "5", true, "sbyte", "5")]
    [TestCase("float", "6.0f", false, "float", "6.0f")]
    [TestCase("unsigned short", "7", true, "ushort", "7")]
    [TestCase("unsigned int", "8", true, "uint", "8")]
    [TestCase("unsigned long long", "9", true, "ulong", "9")]
    [TestCase("unsigned short", "'A'", true, "ushort", "(byte)('A')")]
    public Task OptionalParameterTest(string nativeType, string nativeInit, bool expectedNativeTypeNameAttr, string expectedManagedType, string expectedManagedInit)
    {
        var inputContents = $@"extern ""C"" void MyFunction({nativeType} value = {nativeInit});";

        return ValidateAsync(nameof(OptionalParameterTest), inputContents, discriminator: $"{nativeType}_{nativeInit}_{expectedNativeTypeNameAttr}_{expectedManagedType}_{expectedManagedInit}");
    }

    [TestCase("void *", "nullptr", "void*", "null")]
    [TestCase("void *", "0", "void*", "null")]
    public Task OptionalParameterUnsafeTest(string nativeType, string nativeInit, string expectedManagedType, string expectedManagedInit)
    {
        var inputContents = $@"extern ""C"" void MyFunction({nativeType} value = {nativeInit});";

        return ValidateAsync(nameof(OptionalParameterUnsafeTest), inputContents, discriminator: $"{nativeType}_{nativeInit}_{expectedManagedType}_{expectedManagedInit}");
    }

    [Test]
    public Task WithCallConvTest()
    {
        var inputContents = @"extern ""C"" void MyFunction1(int value); extern ""C"" void MyFunction2(int value);";

        var withCallConvs = new Dictionary<string, string> {
            ["MyFunction1"] = "Winapi"
        };
        return ValidateAsync(nameof(WithCallConvTest), inputContents, withCallConvs: withCallConvs);
    }

    [Test]
    public Task WithCallConvStarTest()
    {
        var inputContents = @"extern ""C"" void MyFunction1(int value); extern ""C"" void MyFunction2(int value);";

        var withCallConvs = new Dictionary<string, string>
        {
            ["*"] = "Winapi"
        };
        return ValidateAsync(nameof(WithCallConvStarTest), inputContents, withCallConvs: withCallConvs);
    }

    [Test]
    public Task WithCallConvStarOverrideTest()
    {
        var inputContents = @"extern ""C"" void MyFunction1(int value); extern ""C"" void MyFunction2(int value);";

        var withCallConvs = new Dictionary<string, string>
        {
            ["*"] = "Winapi",
            ["MyFunction2"] = "StdCall"
        };
        return ValidateAsync(nameof(WithCallConvStarOverrideTest), inputContents, withCallConvs: withCallConvs);
    }

    [Test]
    public Task WithCallConvWildcardTest()
    {
        var inputContents = @"extern ""C"" void MyFunction1(int value); extern ""C"" void MyFunction2(int value); extern ""C"" void MyFunctionExtra(int value);";

        // 'MyFunction?' matches a single trailing character, so it applies to MyFunction1 and
        // MyFunction2 but not MyFunctionExtra.
        var withCallConvs = new Dictionary<string, string>
        {
            ["MyFunction?"] = "Winapi"
        };
        return ValidateAsync(nameof(WithCallConvWildcardTest), inputContents, withCallConvs: withCallConvs);
    }

    [Test]
    public Task WithCallConvWildcardOverrideTest()
    {
        var inputContents = @"extern ""C"" void MyFunction1(int value); extern ""C"" void MyFunction2(int value); extern ""C"" void Other(int value);";

        // Exercises the full precedence chain: the bare '*' catch-all makes everything Winapi, the
        // 'MyFunction*' glob narrows those to StdCall, and the exact 'MyFunction2' key wins over the
        // glob with Cdecl.
        var withCallConvs = new Dictionary<string, string>
        {
            ["*"] = "Winapi",
            ["MyFunction*"] = "StdCall",
            ["MyFunction2"] = "Cdecl"
        };
        return ValidateAsync(nameof(WithCallConvWildcardOverrideTest), inputContents, withCallConvs: withCallConvs);
    }

    [Test]
    public Task WithCallConvStarWildcardNegatedTest()
    {
        var inputContents = @"extern ""C"" void MyFunction1(int value); extern ""C"" void MyFunction2(int value);";

        // '*' applies Winapi to every function; the '*2' glob opt-out opts MyFunction2 back out so it keeps the default.
        var withCallConvs = new Dictionary<string, string>
        {
            ["*"] = "Winapi"
        };
        string[] withoutCallConvs = ["*2"];
        return ValidateAsync(nameof(WithCallConvStarWildcardNegatedTest), inputContents, withCallConvs: withCallConvs, withoutCallConvs: withoutCallConvs);
    }

    [Test]
    public Task WithSetLastErrorTest()
    {
        var inputContents = @"extern ""C"" void MyFunction1(int value); extern ""C"" void MyFunction2(int value);";

        string[] withSetLastErrors = ["MyFunction1"];
        return ValidateAsync(nameof(WithSetLastErrorTest), inputContents, withSetLastErrors: withSetLastErrors);
    }

    [Test]
    public Task WithSetLastErrorStarTest()
    {
        var inputContents = @"extern ""C"" void MyFunction1(int value); extern ""C"" void MyFunction2(int value);";

        string[] withSetLastErrors = ["*"];
        return ValidateAsync(nameof(WithSetLastErrorStarTest), inputContents, withSetLastErrors: withSetLastErrors);
    }

    [Test]
    public Task WithSetLastErrorStarNegatedTest()
    {
        var inputContents = @"extern ""C"" void MyFunction1(int value); extern ""C"" void MyFunction2(int value);";

        // '*' opts every function in; '--without-setlasterror MyFunction2' opts that one back out.
        string[] withSetLastErrors = ["*"];
        string[] withoutSetLastErrors = ["MyFunction2"];
        return ValidateAsync(nameof(WithSetLastErrorStarNegatedTest), inputContents, withSetLastErrors: withSetLastErrors, withoutSetLastErrors: withoutSetLastErrors);
    }

    [Test]
    public Task WithCallConvStarNegatedTest()
    {
        var inputContents = @"extern ""C"" void MyFunction1(int value); extern ""C"" void MyFunction2(int value);";

        // '*' applies Winapi to every function; '--without-callconv MyFunction2' opts that one back out so it keeps the default.
        var withCallConvs = new Dictionary<string, string>
        {
            ["*"] = "Winapi"
        };
        string[] withoutCallConvs = ["MyFunction2"];
        return ValidateAsync(nameof(WithCallConvStarNegatedTest), inputContents, withCallConvs: withCallConvs, withoutCallConvs: withoutCallConvs);
    }

    [Test]
    public Task SourceLocationTest()
        => ValidateAsync(nameof(SourceLocationTest), @"extern ""C"" void MyFunction(float value);", additionalConfigOptions: PInvokeGeneratorConfigurationOptions.GenerateSourceLocationAttribute);

    [Test]
    public Task VarargsTest()
    {
        // Legacy only exercised __arglist output for CSharp on Windows; every Xml variant and every Unix variant
        // stubbed VarargsTestImpl to Task.CompletedTask (no assertion at all). Preserve that coverage exactly here.
        // This is a genuine legacy coverage gap -- surfaced, not silently widened -- since Xml/Unix __arglist output
        // was never asserted (and Unix output cannot be reproduced on a Windows host to seed a baseline).
        if (Variant.Mode != PInvokeGeneratorOutputMode.CSharp || Variant.Os != BaselineOs.Windows)
        {
            return Task.CompletedTask;
        }

        return ValidateAsync(nameof(VarargsTest), @"extern ""C"" void MyFunction(int value, ...);");
    }

    [Test]
    public Task IntrinsicsTest()
        => ValidateAsync(nameof(IntrinsicsTest), @"extern ""C"" void __builtin_cpu_init();
#pragma intrinsic(__builtin_cpu_init)");
}
