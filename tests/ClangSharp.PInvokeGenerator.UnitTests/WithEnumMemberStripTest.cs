// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using ClangSharp.UnitTests.Baseline;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

/// <summary>
/// Tests for the <c>--with-enum-member-strip</c> option, which strips a prefix or suffix from an enum's
/// member names using a per-enum mode (with a <c>*</c> global default). See
/// https://github.com/dotnet/ClangSharp/issues/461.
/// </summary>
[Platform("win")]
public sealed class WithEnumMemberStripTest : StandaloneBaselineTest
{
    protected override string Area => "WithEnumMemberStrip";

    // `common-prefix` auto-detects the longest common prefix (trimmed to the last `_` token boundary) and
    // strips it from every member: `abc_some_enum_first` -> `first`.
    [Test]
    public Task CommonPrefixIsStripped()
    {
        var inputContents = @"enum abc_some_enum
{
    abc_some_enum_first,
    abc_some_enum_second,
    abc_some_enum_third,
};
";

        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(inputContents, withEnumMemberStrip: new Dictionary<string, string> { ["*"] = "common-prefix" });
    }

    // `common-suffix` mirrors `common-prefix` for the "common POSTfix" case: `fast_mode` -> `fast`.
    [Test]
    public Task CommonSuffixIsStripped()
    {
        var inputContents = @"enum render_modes
{
    fast_mode,
    slow_mode,
    idle_mode,
};
";

        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(inputContents, withEnumMemberStrip: new Dictionary<string, string> { ["*"] = "common-suffix" });
    }

    // A strip that would leave a leading digit keeps an underscore so the result stays a valid C# identifier:
    // `abc_version_1_0` -> `_1_0`.
    [Test]
    public Task DigitAfterPrefixIsGuarded()
    {
        var inputContents = @"enum abc_version
{
    abc_version_1_0,
    abc_version_2_0,
};
";

        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(inputContents, withEnumMemberStrip: new Dictionary<string, string> { ["*"] = "common-prefix" });
    }

    // The common prefix/suffix of a single member is the whole name, which is useless, so nothing is stripped.
    [Test]
    public Task SingleMemberIsNotStripped()
    {
        var inputContents = @"enum abc_solo
{
    abc_solo_only,
};
";

        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(inputContents, withEnumMemberStrip: new Dictionary<string, string> { ["*"] = "common-prefix" });
    }

    // Stripping is all-or-nothing: if it would cause two members to collide, the whole enum is left untouched
    // and an informational diagnostic is emitted.
    [Test]
    public Task CollisionAbandonsStripping()
    {
        var inputContents = @"enum abc_flags
{
    abc_flags_1,
    abc_flags__1,
};
";

        var expectedDiagnostics = new Diagnostic[] {
            new Diagnostic(DiagnosticLevel.Info, "Not stripping prefix 'abc_flags_' from enum 'abc_flags' because it would cause member name collisions.", "Line 1, Column 6 in ClangUnsavedFile.h"),
        };

        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(inputContents, withEnumMemberStrip: new Dictionary<string, string> { ["*"] = "common-prefix" }, expectedDiagnostics: expectedDiagnostics);
    }

    // `prefix:<str>` strips an explicit prefix regardless of what the members otherwise share.
    [Test]
    public Task ExplicitPrefixIsStripped()
    {
        var inputContents = @"enum colors
{
    COLOR_RED,
    COLOR_GREEN,
    COLOR_BLUE,
};
";

        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(inputContents, withEnumMemberStrip: new Dictionary<string, string> { ["*"] = "prefix:COLOR_" });
    }

    // `suffix:<str>` strips an explicit suffix.
    [Test]
    public Task ExplicitSuffixIsStripped()
    {
        var inputContents = @"enum access
{
    READ_FLAG,
    WRITE_FLAG,
    EXECUTE_FLAG,
};
";

        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(inputContents, withEnumMemberStrip: new Dictionary<string, string> { ["*"] = "suffix:_FLAG" });
    }

    // `type-name` matches the legacy `strip-enum-member-type-name` behavior: the enum type name is stripped
    // from the beginning of each member.
    [Test]
    public Task TypeNameModeMatchesLegacy()
    {
        var inputContents = @"enum abc_some_enum
{
    abc_some_enum_key1,
    abc_some_enum_key2,
};
";

        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(inputContents, withEnumMemberStrip: new Dictionary<string, string> { ["*"] = "type-name" });
    }

    // A per-enum entry overrides the `*` default: `second_group` opts out of the global `common-prefix`.
    [Test]
    public Task StarDefaultWithPerEnumOverride()
    {
        var inputContents = @"enum first_group
{
    first_group_alpha,
    first_group_beta,
};

enum second_group
{
    second_group_gamma,
    second_group_delta,
};
";

        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(inputContents, withEnumMemberStrip: new Dictionary<string, string> { ["*"] = "common-prefix", ["second_group"] = "none" });
    }

    // Sibling references inside an initializer expression are stripped identically to the declarations, so the
    // generated bindings still compile: `primary = vulkan | metal`.
    [Test]
    public Task SiblingReferencesAreStripped()
    {
        var inputContents = @"enum abc_backend
{
    abc_backend_vulkan = 1 << 0,
    abc_backend_gl = 1 << 1,
    abc_backend_metal = 1 << 2,
    abc_backend_primary = abc_backend_vulkan | abc_backend_metal,
    abc_backend_secondary = abc_backend_gl,
};
";

        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(inputContents, withEnumMemberStrip: new Dictionary<string, string> { ["*"] = "common-prefix" });
    }
}
