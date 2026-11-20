// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

[TestFixture]
public sealed class LargeVirtualMethodCountWithDestructorTest : PInvokeGeneratorTest
{
    [TestCase(190, false)]
    [TestCase(191, false)]
    [TestCase(192, false)]
    [TestCase(191, true)]
    public async Task GeneratesAllVirtualMethods(int methodCount, bool destructorLast)
    {
        var input = new StringBuilder("class Example { public:\n");

        if (!destructorLast)
        {
            _ = input.AppendLine("virtual ~Example() {}");
        }

        for (var i = 0; i < methodCount; i++)
        {
            _ = input.AppendLine(CultureInfo.InvariantCulture, $"virtual bool Method{i}() = 0;");
        }

        if (destructorLast)
        {
            _ = input.AppendLine("virtual ~Example() {}");
        }

        _ = input.AppendLine("};");

        var actual = await GenerateBindingsAsync(
            inputContents: input.ToString(),
            outputMode: PInvokeGeneratorOutputMode.CSharp,
            configOptions: PInvokeGeneratorConfigurationOptions.None,
            excludedNames: null,
            remappedNames: null,
            withAccessSpecifiers: null,
            withAttributes: null,
            withCallConvs: null,
            withClasses: null,
            withLibraryPaths: null,
            withNamespaces: null,
            withSetLastErrors: null,
            withTransparentStructs: null,
            withTypes: null,
            withUsings: null,
            withPackings: null,
            expectedDiagnostics: null,
            libraryPath: DefaultLibraryPath,
            commandLineArgs: null,
            language: "c++",
            languageStandard: DefaultCppStandard).ConfigureAwait(false);

        Assert.That(actual, Does.Contain("public void Dispose()"));

        for (var i = 0; i < methodCount; i++)
        {
            Assert.That(actual, Does.Contain($"public bool Method{i}()"));
        }
    }
}
