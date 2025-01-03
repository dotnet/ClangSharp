// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

[TestFixtureSource(nameof(FixtureArgs))]
public sealed class FunctionDeclarationDllImportTest(PInvokeGeneratorOutputMode outputMode, PInvokeGeneratorConfigurationOptions outputVersion)
    : PInvokeGeneratorTest(outputMode, outputVersion)
{
    private static readonly string[] s_templateTestExcludedNames = ["MyTemplate"];

    [Test]
    public Task BasicTest()
    {
        var inputContents = @"extern ""C"" void MyFunction();";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task ArrayParameterTest()
    {
        var inputContents = @"extern ""C"" void MyFunction(const float color[4]);";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task FunctionPointerParameterTest()
    {
        var inputContents = @"extern ""C"" void MyFunction(void (*callback)());";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task NamespaceTest()
    {
        var inputContents = @"namespace MyNamespace
{
    void MyFunction();
}";

        return ValidateGeneratedBindingsAsync(inputContents, osxTransform: (output) => output.Replace("__ZN11MyNamespace10MyFunctionEv", "_ZN11MyNamespace10MyFunctionEv", System.StringComparison.Ordinal));
    }

    [TestCase("int")]
    [TestCase("bool")]
    [TestCase("float *")]
    [TestCase("void (*)(int)")]
    public Task TemplateParameterTest(string nativeType)
    {
        var inputContents = @$"template <typename T> struct MyTemplate;

extern ""C"" void MyFunction(MyTemplate<{nativeType}> myStruct);";

        return ValidateGeneratedBindingsAsync(inputContents, excludedNames: s_templateTestExcludedNames);
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

        return ValidateGeneratedBindingsAsync(inputContents, excludedNames: s_templateTestExcludedNames);
    }

    [Test]
    public Task NoLibraryPathTest()
    {
        var inputContents = @"extern ""C"" void MyFunction();";

        return ValidateGeneratedBindingsAsync(inputContents, libraryPath: string.Empty);
    }

    [Test]
    public Task WithLibraryPathTest()
    {
        var inputContents = @"extern ""C"" void MyFunction();";

        var withLibraryPaths = new Dictionary<string, string>
        {
            ["MyFunction"] = "ClangSharpPInvokeGenerator"
        };
        return ValidateGeneratedBindingsAsync(inputContents, libraryPath: string.Empty, withLibraryPaths: withLibraryPaths);
    }

    [Test]
    public Task WithLibraryPathStarTest()
    {
        var inputContents = @"extern ""C"" void MyFunction();";

        var withLibraryPaths = new Dictionary<string, string>
        {
            ["*"] = "ClangSharpPInvokeGenerator"
        };
        return ValidateGeneratedBindingsAsync(inputContents, libraryPath: string.Empty, withLibraryPaths: withLibraryPaths);
    }

    [TestCase("unsigned char", "0")]
    [TestCase("double", "1.0")]
    [TestCase("short", "2")]
    [TestCase("int", "3")]
    [TestCase("long long", "4")]
    [TestCase("signed char", "5")]
    [TestCase("float", "6.0f")]
    [TestCase("unsigned short", "7")]
    [TestCase("unsigned int", "8")]
    [TestCase("unsigned long long", "9")]
    [TestCase("unsigned short", "'A'")]
    public Task OptionalParameterTest(string nativeType, string nativeInit)
    {
        var inputContents = $@"extern ""C"" void MyFunction({nativeType} value = {nativeInit});";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [TestCase("void *", "nullptr")]
    [TestCase("void *", "0")]
    public Task OptionalParameterUnsafeTest(string nativeType, string nativeInit)
    {
        var inputContents = $@"extern ""C"" void MyFunction({nativeType} value = {nativeInit});";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task WithCallConvTest()
    {
        var inputContents = @"extern ""C"" void MyFunction1(int value); extern ""C"" void MyFunction2(int value);";

        var withCallConvs = new Dictionary<string, string> {
            ["MyFunction1"] = "Winapi"
        };
        return ValidateGeneratedBindingsAsync(inputContents, withCallConvs: withCallConvs);
    }

    [Test]
    public Task WithCallConvStarTest()
    {
        var inputContents = @"extern ""C"" void MyFunction1(int value); extern ""C"" void MyFunction2(int value);";

        var withCallConvs = new Dictionary<string, string>
        {
            ["*"] = "Winapi"
        };
        return ValidateGeneratedBindingsAsync(inputContents, withCallConvs: withCallConvs);
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
        return ValidateGeneratedBindingsAsync(inputContents, withCallConvs: withCallConvs);
    }

    [Test]
    public Task WithSetLastErrorTest()
    {
        var inputContents = @"extern ""C"" void MyFunction1(int value); extern ""C"" void MyFunction2(int value);";

        var withSetLastErrors = new string[]
        {
            "MyFunction1"
        };
        return ValidateGeneratedBindingsAsync(inputContents, withSetLastErrors: withSetLastErrors);
    }

    [Test]
    public Task WithSetLastErrorStarTest()
    {
        var inputContents = @"extern ""C"" void MyFunction1(int value); extern ""C"" void MyFunction2(int value);";

        var withSetLastErrors = new string[]
        {
            "*"
        };
        return ValidateGeneratedBindingsAsync(inputContents, withSetLastErrors: withSetLastErrors);
    }

    [Test]
    public Task SourceLocationTest()
    {
        const string InputContents = @"extern ""C"" void MyFunction(float value);";

        return ValidateGeneratedBindingsAsync(InputContents, PInvokeGeneratorConfigurationOptions.GenerateSourceLocationAttribute);
    }

    [Test]
    public Task VarargsTest() => Task.CompletedTask;

    [Test]
    public Task IntrinsicsTest()
    {
        const string InputContents = @"extern ""C"" void __builtin_cpu_init();
#pragma intrinsic(__builtin_cpu_init)";

        return ValidateGeneratedBindingsAsync(InputContents);
    }
}
