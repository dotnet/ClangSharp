// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Threading.Tasks;
using ClangSharp.UnitTests.Baseline;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

/// <summary>
/// Regression tests for https://github.com/dotnet/ClangSharp/issues/657 and
/// https://github.com/dotnet/ClangSharp/issues/429.
/// When <c>generate-helper-types</c> is used with a single output file, the helper types must be
/// emitted inside the shared namespace with their usings hoisted to the top of the file, rather than
/// each helper emitting its own <c>using</c> directives and <c>namespace</c> wrapper (which produced
/// invalid C# with nested namespaces, usings after a namespace, and a stray trailing brace). The
/// helper types must also be emitted at the bottom of the shared namespace, after the deferred method
/// class, rather than in the middle of the generated output.
/// </summary>
[Platform("win")]
public sealed class HelperTypesTest : StandaloneBaselineTest
{
    protected override string Area => "HelperTypes";

    [Test]
    public Task HelperTypesEmittedInSharedNamespace()
    {
        var inputContents = @"struct SRC_DATA
{
    const float *data_in;
    long input_frames;
};
";

        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(inputContents, additionalConfigOptions: PInvokeGeneratorConfigurationOptions.GenerateHelperTypes);
    }

    [Test]
    public Task HelperTypesEmittedInSharedFileScopedNamespace()
    {
        var inputContents = @"struct SRC_DATA
{
    const float *data_in;
    long input_frames;
};
";

        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(inputContents, additionalConfigOptions: PInvokeGeneratorConfigurationOptions.GenerateHelperTypes | PInvokeGeneratorConfigurationOptions.GenerateFileScopedNamespaces);
    }

    [Test]
    public Task HelperTypesEmittedAfterMethodClass()
    {
        var inputContents = @"struct SRC_DATA
{
    const float *data_in;
    long input_frames;
};

extern ""C"" void MyFunction();
";

        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(inputContents, additionalConfigOptions: PInvokeGeneratorConfigurationOptions.GenerateHelperTypes);
    }

    [Test]
    public Task HelperTypesEmittedAfterMethodClassFileScoped()
    {
        var inputContents = @"struct SRC_DATA
{
    const float *data_in;
    long input_frames;
};

extern ""C"" void MyFunction();
";

        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(inputContents, additionalConfigOptions: PInvokeGeneratorConfigurationOptions.GenerateHelperTypes | PInvokeGeneratorConfigurationOptions.GenerateFileScopedNamespaces);
    }
}
