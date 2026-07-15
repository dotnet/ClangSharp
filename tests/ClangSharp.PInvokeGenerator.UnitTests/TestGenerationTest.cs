// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

/// <summary>
/// Coverage for the generator's test-generation code paths (https://github.com/dotnet/ClangSharp/issues/223).
/// Passing <c>GenerateTestsNUnit</c>/<c>GenerateTestsXUnit</c> together with a test output location makes the
/// generator emit a companion validation file (struct size/blittability/layout, GUID checks, ...). None of the
/// existing fixtures exercised these paths, so this pins the emitted NUnit and xUnit output for the common shapes.
/// </summary>
public sealed class TestGenerationTest : PInvokeGeneratorTest
{
    private const string StructInputContents = @"struct MyStruct
{
    int x;
    int y;
};
";

    // __declspec(uuid(...)) only parses on a Windows target triple, matching the other GUID fixtures.
    private const string GuidStructInputContents = @"#define DECLSPEC_UUID(x) __declspec(uuid(x))

struct DECLSPEC_UUID(""00000000-0000-0000-C000-000000000046"") MyStruct1
{
    int x;
};
";

    private static readonly string[] s_declspecExcludedNames = ["DECLSPEC_UUID"];

    [Test]
    public Task StructTestsNUnit()
    {
        var expectedTestOutputContents = @"using NUnit.Framework;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        public int x;

        public int y;
    }

    /// <summary>Provides validation of the <see cref=""MyStruct"" /> struct.</summary>
    public static unsafe partial class MyStructTests
    {
        /// <summary>Validates that the <see cref=""MyStruct"" /> struct is blittable.</summary>
        [Test]
        public static void IsBlittableTest()
        {
            Assert.That(Marshal.SizeOf<MyStruct>(), Is.EqualTo(sizeof(MyStruct)));
        }

        /// <summary>Validates that the <see cref=""MyStruct"" /> struct has the right <see cref=""LayoutKind"" />.</summary>
        [Test]
        public static void IsLayoutSequentialTest()
        {
            Assert.That(typeof(MyStruct).IsLayoutSequential, Is.True);
        }

        /// <summary>Validates that the <see cref=""MyStruct"" /> struct has the correct size.</summary>
        [Test]
        public static void SizeOfTest()
        {
            Assert.That(sizeof(MyStruct), Is.EqualTo(8));
        }
    }
}
";

        return ValidateGeneratedTestBindingsAsync(StructInputContents, expectedTestOutputContents, PInvokeGeneratorConfigurationOptions.GenerateTestsNUnit);
    }

    [Test]
    public Task StructTestsXUnit()
    {
        var expectedTestOutputContents = @"using System.Runtime.InteropServices;
using Xunit;

namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        public int x;

        public int y;
    }

    /// <summary>Provides validation of the <see cref=""MyStruct"" /> struct.</summary>
    public static unsafe partial class MyStructTests
    {
        /// <summary>Validates that the <see cref=""MyStruct"" /> struct is blittable.</summary>
        [Fact]
        public static void IsBlittableTest()
        {
            Assert.Equal(sizeof(MyStruct), Marshal.SizeOf<MyStruct>());
        }

        /// <summary>Validates that the <see cref=""MyStruct"" /> struct has the right <see cref=""LayoutKind"" />.</summary>
        [Fact]
        public static void IsLayoutSequentialTest()
        {
            Assert.True(typeof(MyStruct).IsLayoutSequential);
        }

        /// <summary>Validates that the <see cref=""MyStruct"" /> struct has the correct size.</summary>
        [Fact]
        public static void SizeOfTest()
        {
            Assert.Equal(8, sizeof(MyStruct));
        }
    }
}
";

        return ValidateGeneratedTestBindingsAsync(StructInputContents, expectedTestOutputContents, PInvokeGeneratorConfigurationOptions.GenerateTestsXUnit);
    }

    [Test]
    [Platform("win")]
    public Task GuidStructTestsNUnit()
    {
        var expectedTestOutputContents = @"using NUnit.Framework;
using System;
using System.Runtime.InteropServices;
using static ClangSharp.Test.Methods;

namespace ClangSharp.Test
{
    [Guid(""00000000-0000-0000-C000-000000000046"")]
    public partial struct MyStruct1
    {
        public int x;
    }

    /// <summary>Provides validation of the <see cref=""MyStruct1"" /> struct.</summary>
    public static unsafe partial class MyStruct1Tests
    {
        /// <summary>Validates that the <see cref=""Guid"" /> of the <see cref=""MyStruct1"" /> struct is correct.</summary>
        [Test]
        public static void GuidOfTest()
        {
            Assert.That(typeof(MyStruct1).GUID, Is.EqualTo(IID_MyStruct1));
        }

        /// <summary>Validates that the <see cref=""MyStruct1"" /> struct is blittable.</summary>
        [Test]
        public static void IsBlittableTest()
        {
            Assert.That(Marshal.SizeOf<MyStruct1>(), Is.EqualTo(sizeof(MyStruct1)));
        }

        /// <summary>Validates that the <see cref=""MyStruct1"" /> struct has the right <see cref=""LayoutKind"" />.</summary>
        [Test]
        public static void IsLayoutSequentialTest()
        {
            Assert.That(typeof(MyStruct1).IsLayoutSequential, Is.True);
        }

        /// <summary>Validates that the <see cref=""MyStruct1"" /> struct has the correct size.</summary>
        [Test]
        public static void SizeOfTest()
        {
            Assert.That(sizeof(MyStruct1), Is.EqualTo(4));
        }
    }
}
";

        return ValidateGeneratedTestBindingsAsync(GuidStructInputContents, expectedTestOutputContents, PInvokeGeneratorConfigurationOptions.GenerateTestsNUnit, excludedNames: s_declspecExcludedNames);
    }

    [Test]
    [Platform("win")]
    public Task GuidStructTestsXUnit()
    {
        var expectedTestOutputContents = @"using System;
using System.Runtime.InteropServices;
using Xunit;
using static ClangSharp.Test.Methods;

namespace ClangSharp.Test
{
    [Guid(""00000000-0000-0000-C000-000000000046"")]
    public partial struct MyStruct1
    {
        public int x;
    }

    /// <summary>Provides validation of the <see cref=""MyStruct1"" /> struct.</summary>
    public static unsafe partial class MyStruct1Tests
    {
        /// <summary>Validates that the <see cref=""Guid"" /> of the <see cref=""MyStruct1"" /> struct is correct.</summary>
        [Fact]
        public static void GuidOfTest()
        {
            Assert.Equal(typeof(MyStruct1).GUID, IID_MyStruct1);
        }

        /// <summary>Validates that the <see cref=""MyStruct1"" /> struct is blittable.</summary>
        [Fact]
        public static void IsBlittableTest()
        {
            Assert.Equal(sizeof(MyStruct1), Marshal.SizeOf<MyStruct1>());
        }

        /// <summary>Validates that the <see cref=""MyStruct1"" /> struct has the right <see cref=""LayoutKind"" />.</summary>
        [Fact]
        public static void IsLayoutSequentialTest()
        {
            Assert.True(typeof(MyStruct1).IsLayoutSequential);
        }

        /// <summary>Validates that the <see cref=""MyStruct1"" /> struct has the correct size.</summary>
        [Fact]
        public static void SizeOfTest()
        {
            Assert.Equal(4, sizeof(MyStruct1));
        }
    }
}
";

        return ValidateGeneratedTestBindingsAsync(GuidStructInputContents, expectedTestOutputContents, PInvokeGeneratorConfigurationOptions.GenerateTestsXUnit, excludedNames: s_declspecExcludedNames);
    }

    // With neither test-generation option set the generator emits no companion test file, so the captured test
    // output is empty. This guards the default path and ensures enabling tests is what drives the emission.
    [Test]
    public async Task NoTestsGeneratedWhenNotRequested()
    {
        var (_, testOutputContents) = await GenerateBindingsWithTestOutputAsync(StructInputContents, PInvokeGeneratorOutputMode.CSharp, PInvokeGeneratorConfigurationOptions.GenerateLatestCode, excludedNames: null, remappedNames: null, withAccessSpecifiers: null, withAttributes: null, withCallConvs: null, withClasses: null, withLibraryPaths: null, withNamespaces: null, withSetLastErrors: null, withTransparentStructs: null, withTypes: null, withUsings: null, withPackings: null, expectedDiagnostics: null, libraryPath: DefaultLibraryPath, commandLineArgs: null, language: "c++", languageStandard: DefaultCppStandard).ConfigureAwait(false);
        Assert.That(testOutputContents, Is.Empty);
    }

    private static async Task ValidateGeneratedTestBindingsAsync(string inputContents, string expectedTestOutputContents, PInvokeGeneratorConfigurationOptions additionalConfigOptions, string[]? excludedNames = null)
    {
        var configOptions = PInvokeGeneratorConfigurationOptions.GenerateLatestCode | additionalConfigOptions;
        var (_, testOutputContents) = await GenerateBindingsWithTestOutputAsync(inputContents, PInvokeGeneratorOutputMode.CSharp, configOptions, excludedNames, remappedNames: null, withAccessSpecifiers: null, withAttributes: null, withCallConvs: null, withClasses: null, withLibraryPaths: null, withNamespaces: null, withSetLastErrors: null, withTransparentStructs: null, withTypes: null, withUsings: null, withPackings: null, expectedDiagnostics: null, libraryPath: DefaultLibraryPath, commandLineArgs: null, language: "c++", languageStandard: DefaultCppStandard).ConfigureAwait(false);
        Assert.That(testOutputContents.ReplaceLineEndings("\n"), Is.EqualTo(expectedTestOutputContents.ReplaceLineEndings("\n")));
    }
}
