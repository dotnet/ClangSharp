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

    [Test]
    public Task NestedIneligibleLayoutIsTransitive()
    {
        // Inner contains a fixed buffer, so it is not field-wise comparable. Outer embeds Inner by value,
        // so Outer must also be left untouched rather than deferring to reflection-based ValueType.Equals.
        var inputContents = @"struct Inner
{
    int Values[4];
};

struct Outer
{
    int Id;
    Inner Data;
};
";

        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(inputContents, additionalConfigOptions: PInvokeGeneratorConfigurationOptions.GenerateEqualityMethods);
    }

    [Test]
    public Task BitfieldsCompareBackingStorage()
    {
        // The individual bitfield regions are accessor properties over shared integer backing fields, so
        // a field-wise comparison compares `_bitfield` (and `_bitfield1`, ...) rather than each region.
        var inputContents = @"struct Flags
{
    unsigned int A : 1;
    unsigned int B : 2;
    unsigned int C : 3;
    int Value;
};
";

        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(inputContents, additionalConfigOptions: PInvokeGeneratorConfigurationOptions.GenerateEqualityMethods);
    }

    [Test]
    public Task BaseSubobjectsAreCompared()
    {
        // Non-polymorphic inheritance is emitted as a base subobject field, so equality compares that
        // base field and then the derived struct's own fields.
        var inputContents = @"struct Base
{
    int X;
    int Y;
};

struct Derived : Base
{
    int Z;
};
";

        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(inputContents, additionalConfigOptions: PInvokeGeneratorConfigurationOptions.GenerateEqualityMethods);
    }

    [Test]
    public Task AnonymousStructFieldsAreCompared()
    {
        // The anonymous struct is promoted to a nested record plus a backing field; comparing that field
        // compares every promoted member exactly once.
        var inputContents = @"struct Outer
{
    int Id;
    struct
    {
        int First;
        int Second;
    };
};
";

        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(inputContents, additionalConfigOptions: PInvokeGeneratorConfigurationOptions.GenerateEqualityMethods);
    }

    [Test]
    public Task AnonymousUnionFieldsAreLeftUntouched()
    {
        // An anonymous union nests overlapping storage, so the containing struct is not field-wise
        // comparable and must be left untouched.
        var inputContents = @"struct Outer
{
    int Id;
    union
    {
        int AsInt;
        float AsFloat;
    };
};
";

        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(inputContents, additionalConfigOptions: PInvokeGeneratorConfigurationOptions.GenerateEqualityMethods);
    }
}
