// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests.Baseline;

[TestFixtureSource(nameof(Variants))]
public sealed class LowercaseTypeNameEscapeTest : BaselineTest
{
    private static readonly Dictionary<string, string> LowercaseClass = new() { ["MyValue"] = "clang" };

    public LowercaseTypeNameEscapeTest(BaselineVariant variant) : base(variant)
    {
    }

    protected override string Area => "LowercaseTypeNameEscape";

    // A top-level type whose name is composed solely of lowercase ASCII letters (e.g. `clang`) collides
    // with C#'s reservation of such identifiers for future contextual keywords (CS8981); the declaration
    // must be `@`-escaped so it compiles without a warning.
    [Test]
    public Task LowercaseClassNameEscapedTest()
        => ValidateAsync(nameof(LowercaseClassNameEscapedTest), @"constexpr int MyValue = 1;
", withClasses: LowercaseClass);

    // The escape must apply to every type name, not just the top-level method class: a lowercase-only
    // record declaration and every reference to it both resolve through `GetRemappedCursorName`, so both
    // must emit the `@`-escaped name. Members keep their (lowercase) names unescaped, since CS8981 only
    // applies to type names. Finally, a `--with-access-specifier` keyed on the raw (un-escaped) name must
    // still apply, proving the implicit escape doesn't cause the user's config to be missed.
    [Test]
    public Task LowercaseTypeReferenceEscapedTest()
    {
        var inputContents = @"struct config
{
    int value;
};

struct holder
{
    struct config* cfg;
};
";

        var withAccessSpecifiers = new Dictionary<string, AccessSpecifier> {
            ["config"] = AccessSpecifier.Internal,
        };

        return ValidateAsync(nameof(LowercaseTypeReferenceEscapedTest), inputContents, withAccessSpecifiers: withAccessSpecifiers);
    }
}
