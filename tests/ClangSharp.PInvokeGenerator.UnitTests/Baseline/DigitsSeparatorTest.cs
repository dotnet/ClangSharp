// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests.Baseline;

// Proof-of-concept: DigitsSeparatorTest migrated to the baseline model. A second area demonstrating the
// harness generalizes across the repo's two divergent fixture-naming conventions (this area is
// namespace-scoped, VarDeclaration is name-prefixed) and multi-argument TestCases whose first argument is
// not a unique discriminator.
[TestFixtureSource(nameof(Variants))]
public sealed class DigitsSeparatorTest : BaselineTest
{
    public DigitsSeparatorTest(BaselineVariant variant) : base(variant)
    {
    }

    protected override string Area => "DigitsSeparator";

    [TestCase("int", "1'024", "1_024")]
    [TestCase("int", "1'000'001", "1_000_001")]
    [TestCase("float", "1'024", "1_024")]
    [TestCase("float", "1'024.0", "1_024.0")]
    public Task StaticConstExprTest(string type, string nativeValue, string expectedValue)
    {
        var inputContents = $@"class MyClass
{{
    private:

      static constexpr {type} x = {nativeValue};
}};
";

        return ValidateAsync(nameof(StaticConstExprTest), inputContents, discriminator: $"{type}_{nativeValue}_{expectedValue}");
    }
}
