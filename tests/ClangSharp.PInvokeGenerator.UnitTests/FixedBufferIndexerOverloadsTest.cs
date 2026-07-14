// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Threading.Tasks;
using ClangSharp.UnitTests.Baseline;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

/// <summary>
/// Tests for https://github.com/dotnet/ClangSharp/issues/450.
/// When <c>generate-fixed-buffer-indexer-overloads</c> is set, the generated <c>_e__FixedBuffer</c>
/// helper struct should expose additional <c>uint</c>, <c>nint</c>, and <c>nuint</c> indexers alongside
/// the default <c>int</c> one so callers can index without casting. Without the switch, only the
/// <c>int</c> indexer is generated.
/// </summary>
[Platform("win")]
public sealed class FixedBufferIndexerOverloadsTest : StandaloneBaselineTest
{
    protected override string Area => "FixedBufferIndexerOverloads";

    [Test]
    public Task GeneratesOverloadsWhenEnabled()
    {
        var inputContents = @"struct MyStruct
{
    int value;
};

struct MyOtherStruct
{
    MyStruct c[3];
};
";

        return ValidateGeneratedCSharpCompatibleWindowsBaselineAsync(inputContents, additionalConfigOptions: PInvokeGeneratorConfigurationOptions.GenerateFixedBufferIndexerOverloads);
    }

    [Test]
    public Task GeneratesOnlyIntIndexerWhenDisabled()
    {
        var inputContents = @"struct MyStruct
{
    int value;
};

struct MyOtherStruct
{
    MyStruct c[3];
};
";

        return ValidateGeneratedCSharpCompatibleWindowsBaselineAsync(inputContents);
    }
}
