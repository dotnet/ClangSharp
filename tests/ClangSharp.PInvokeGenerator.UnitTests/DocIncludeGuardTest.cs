// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

[TestFixture]
public sealed class DocIncludeGuardTest : PInvokeGeneratorTest
{
    [Test]
    public async Task EmptyRemappedNameDoesNotEmitDocInclude()
    {
        var remappedNames = new Dictionary<string, string> { ["MY_MACRO"] = "" };

        var actual = await GenerateBindingsAsync(
            inputContents: "#define MY_MACRO 42",
            outputMode: PInvokeGeneratorOutputMode.CSharp,
            configOptions: PInvokeGeneratorConfigurationOptions.GenerateDocIncludes,
            excludedNames: null,
            remappedNames: remappedNames,
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

        Assert.That(actual, Does.Contain("[NativeTypeName(\"#define MY_MACRO 42\")]"));
        Assert.That(actual, Does.Not.Contain("/// <include"));
    }
}
