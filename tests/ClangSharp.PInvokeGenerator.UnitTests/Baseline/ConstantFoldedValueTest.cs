// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests.Baseline;

[TestFixtureSource(nameof(Variants))]
public sealed class ConstantFoldedValueTest : BaselineTest
{
    private static readonly string[] CompanionExcludedNames = ["Companion"];
    private static readonly string[] TargetFoldedValues = ["Target.*"];
    private static readonly string[] ValueFoldedValues = ["MyValue"];
    private static readonly string[] StarFoldedValues = ["*"];
    private static readonly string[] TargetKeepOptOut = ["Target.Target_Keep"];

    public ConstantFoldedValueTest(BaselineVariant variant) : base(variant)
    {
    }

    protected override string Area => "ConstantFoldedValue";

    // An enum member whose initializer references an excluded companion enum folds to the clang-evaluated
    // literal, so no reference to (and no 'using static' for) the excluded enum is emitted.
    [Test]
    public Task ExcludedEnumFoldTest()
        => ValidateAsync(nameof(ExcludedEnumFoldTest), @"enum Companion : int
{
    Companion_A,
    Companion_B,
};

enum Target : int
{
    Target_X = static_cast<int>(Companion_B) + 1,
};
", excludedNames: CompanionExcludedNames, withConstantFoldedValues: TargetFoldedValues);

    // A const value declaration whose initializer references an excluded companion enum folds to a literal.
    [Test]
    public Task ConstValueFoldTest()
        => ValidateAsync(nameof(ConstValueFoldTest), @"enum Companion : int
{
    Companion_A = 40,
};

constexpr int MyValue = static_cast<int>(Companion_A) + 2;
", excludedNames: CompanionExcludedNames, withConstantFoldedValues: ValueFoldedValues);

    // '--with-constant-folded-value *' folds every enum member, while '--without-constant-folded-value'
    // opts a specific member back out of the catch-all so it keeps its written initializer.
    [Test]
    public Task WildcardOptOutTest()
        => ValidateAsync(nameof(WildcardOptOutTest), @"enum Target : int
{
    Target_Fold = 1 + 1,
    Target_Keep = 2 + 2,
};
", withConstantFoldedValues: StarFoldedValues, withoutConstantFoldedValues: TargetKeepOptOut);
}
