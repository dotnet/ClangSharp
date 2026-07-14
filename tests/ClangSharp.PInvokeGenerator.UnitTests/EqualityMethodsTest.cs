// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Threading.Tasks;
using ClangSharp.UnitTests.Baseline;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

/// <summary>
/// Regression tests for https://github.com/dotnet/ClangSharp/issues/539.
/// The opt-in <c>generate-equality-methods</c> option makes generated structs implement
/// <c>IEquatable&lt;T&gt;</c> with a field-wise <c>Equals</c>, a matching <c>GetHashCode</c>, and the
/// <c>==</c> / <c>!=</c> operators. This is strictly opt-in because element-wise comparison is not valid
/// for every native type. Records that cannot be compared field-for-field (unions, vtbl/COM types,
/// bitfields, fixed buffers, anonymous fields) must be left untouched.
/// </summary>
[Platform("win")]
public sealed class EqualityMethodsTest : StandaloneBaselineTest
{
    protected override string Area => "EqualityMethods";

    [Test]
    public Task ScalarPointerAndNestedFields()
    {
        var inputContents = @"struct Point
{
    int X;
    int Y;
};

struct MyData
{
    int Value;
    float Ratio;
    Point Origin;
    void* Handle;
};
";

        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(inputContents, additionalConfigOptions: PInvokeGeneratorConfigurationOptions.GenerateEqualityMethods);
    }

    [Test]
    public Task UnionsAreLeftUntouched()
    {
        var inputContents = @"union MyUnion
{
    int AsInt;
    float AsFloat;
};
";

        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(inputContents, additionalConfigOptions: PInvokeGeneratorConfigurationOptions.GenerateEqualityMethods);
    }

    [Test]
    public Task FixedBuffersAreLeftUntouched()
    {
        var inputContents = @"struct WithArray
{
    int Values[4];
};
";

        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(inputContents, additionalConfigOptions: PInvokeGeneratorConfigurationOptions.GenerateEqualityMethods);
    }
}
