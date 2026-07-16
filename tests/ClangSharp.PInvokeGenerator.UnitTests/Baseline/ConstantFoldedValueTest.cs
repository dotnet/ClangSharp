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
    private static readonly string[] TargetLastFoldedValues = ["Target.Target_Last"];

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

    // Even when folding is requested, an initializer that is purely an alias to (or an `|` combination
    // of) sibling enumerators stays symbolic -- those references are themselves generated, so keeping
    // them readable (e.g. range markers) is preferable. Only non-alias arithmetic is folded.
    [Test]
    public Task EnumeratorAliasPreservedTest()
        => ValidateAsync(nameof(EnumeratorAliasPreservedTest), @"enum Target : int
{
    Target_A,
    Target_B,
    Target_First = Target_A,
    Target_Combined = Target_A | Target_B,
    Target_Computed = Target_B + 1,
};
", withConstantFoldedValues: StarFoldedValues);

    // A member whose initializer is opaque arithmetic (not an alias `DeclRefExpr`) but whose folded value
    // equals a prior sibling emits that sibling symbolically rather than the literal -- e.g. `_Last` range
    // markers built from an X-macro count. Only prior siblings are considered.
    [Test]
    public Task FoldedValueAliasesSiblingTest()
        => ValidateAsync(nameof(FoldedValueAliasesSiblingTest), @"enum Target : int
{
    Target_A,
    Target_B,
    Target_C,
    Target_Last = 1 + 1,
};
", withConstantFoldedValues: TargetLastFoldedValues);
}
