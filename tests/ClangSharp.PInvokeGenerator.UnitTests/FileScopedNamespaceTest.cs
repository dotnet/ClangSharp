// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Threading.Tasks;
using ClangSharp.UnitTests.Baseline;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

/// <summary>
/// Regression test for https://github.com/dotnet/ClangSharp/issues/555.
/// When <c>generate-file-scoped-namespaces</c> is used with a single output file, no namespace
/// opening brace is emitted, so no closing brace must be emitted either. Members after the first
/// must also not be indented as if they were nested inside a namespace block.
/// </summary>
[Platform("win")]
public sealed class FileScopedNamespaceTest : StandaloneBaselineTest
{
    protected override string Area => "FileScopedNamespace";

    [Test]
    public Task MethodClassHasNoTrailingBrace()
    {
        var inputContents = @"extern ""C"" void MyFunction();
";

        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(inputContents, PInvokeGeneratorConfigurationOptions.GenerateFileScopedNamespaces);
    }

    [Test]
    public Task StructHasNoTrailingBrace()
    {
        var inputContents = @"struct Point
{
    int x;
    int y;
};
";

        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(inputContents, PInvokeGeneratorConfigurationOptions.GenerateFileScopedNamespaces);
    }

    [Test]
    public Task MembersAfterFirstAreNotOverIndented()
    {
        var inputContents = @"struct Point
{
    int x;
    int y;
};

enum Color
{
    Red,
    Green,
};

extern ""C"" void MyFunction();
";

        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(inputContents, PInvokeGeneratorConfigurationOptions.GenerateFileScopedNamespaces);
    }
}
