// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests.Baseline;

// Shared baseline read/write core used by both the matrix-parameterized BaselineTest and the
// single-variant StandaloneBaselineTest. Given the already-generated output for a case+variant, it either
// (re)writes the checked-in baseline under UPDATE_BASELINES, or resolves the baseline via the fallback
// chain and asserts equality after resolving the generator version placeholder.
internal static class BaselineAssertions
{
    private const string GeneratedCodeAttributePlaceholder = "GeneratedCode(\"ClangSharp\", \"{ClangSharpVersion}\")";
    private static readonly string s_generatedCodeAttribute = $"GeneratedCode(\"ClangSharp\", \"{typeof(PInvokeGenerator).Assembly.GetName().Version}\")";

    internal static string NormalizeGeneratedCodeVersion(string actual)
        => actual.Replace(s_generatedCodeAttribute, GeneratedCodeAttributePlaceholder, StringComparison.Ordinal);

    internal static string ResolveGeneratedCodeVersion(string expected)
        => expected.Replace(GeneratedCodeAttributePlaceholder, s_generatedCodeAttribute, StringComparison.Ordinal);

    public static async Task AssertOrUpdateAsync(string area, string caseName, BaselineVariant variant, string actual)
    {
        if (BaselineHarness.ShouldUpdateBaselines)
        {
            var path = BaselineHarness.MostSpecificPath(area, caseName, variant);
            _ = Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            await File.WriteAllTextAsync(path, NormalizeGeneratedCodeVersion(actual)).ConfigureAwait(false);
            return;
        }

        var resolved = BaselineHarness.ResolveBaselinePath(area, caseName, variant);
        Assert.That(resolved, Is.Not.Null, $"No baseline found for case '{caseName}' variant '{variant}'. Run with UPDATE_BASELINES=1 (Windows variants) or seed from the legacy tests (Unix variants).");

        var expected = await File.ReadAllTextAsync(resolved!).ConfigureAwait(false);
        Assert.That(actual, Is.EqualTo(ResolveGeneratedCodeVersion(expected)));
    }
}
