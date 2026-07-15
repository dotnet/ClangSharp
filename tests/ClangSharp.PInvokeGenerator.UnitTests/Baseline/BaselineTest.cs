// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests.Baseline;

// Base class for migrated, baseline-driven fixtures. A single fixture is parameterized over all 16
// (Mode, Config, OS) variants via [TestFixtureSource]; each concrete test supplies the C/C++ input and a
// case name exactly once, and the expected output comes from a checked-in baseline file.
public abstract class BaselineTest : PInvokeGeneratorTest
{
    private readonly BaselineVariant _variant;

    protected BaselineTest(BaselineVariant variant)
    {
        _variant = variant;
    }

    protected abstract string Area { get; }

    // The variant this fixture instance is exercising. Exposed so a migrated case can replicate a legacy no-op
    // (a folder that stubbed its *Impl to Task.CompletedTask) for the specific variants it was never run under.
    protected BaselineVariant Variant => _variant;

    // Feeds [TestFixtureSource]; Unix variants are ignored on Windows hosts (and vice versa), matching the
    // legacy [Platform("win")] / [Platform("unix")] gating exactly.
    protected static IEnumerable<TestFixtureData> Variants()
    {
        foreach (var variant in BaselineHarness.AllVariants)
        {
            var data = new TestFixtureData(variant);
            _ = data.SetArgDisplayNames(variant.ToString());

            if (!variant.MatchesHost && !BaselineHarness.RunAllVariants)
            {
                _ = data.Ignore($"{variant.Os} variant only runs on matching hosts");
            }

            yield return data;
        }
    }

    // Mirrors the full ValidateGeneratedCSharpLatestWindowsBindingsAsync surface (minus expectedOutputContents,
    // which now lives in a checked-in baseline) so any migrated area can express its options exactly. The
    // Mode/Config come from the fixture variant; additionalConfigOptions OR-in the area's positional flags.
    protected Task ValidateAsync(string method, string inputContents, string? discriminator = null, PInvokeGeneratorConfigurationOptions additionalConfigOptions = PInvokeGeneratorConfigurationOptions.None, string[]? excludedNames = null, IReadOnlyDictionary<string, string>? remappedNames = null, IReadOnlyDictionary<string, AccessSpecifier>? withAccessSpecifiers = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withAttributes = null, IReadOnlyDictionary<string, string>? withCallConvs = null, IReadOnlyDictionary<string, string>? withClasses = null, IReadOnlyDictionary<string, string>? withLibraryPaths = null, IReadOnlyDictionary<string, string>? withNamespaces = null, string[]? withSetLastErrors = null, IReadOnlyDictionary<string, (string, PInvokeGeneratorTransparentStructKind)>? withTransparentStructs = null, IReadOnlyDictionary<string, string>? withTypes = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withUsings = null, IReadOnlyDictionary<string, string>? withPackings = null, IEnumerable<Diagnostic>? expectedDiagnostics = null, string libraryPath = DefaultLibraryPath, string[]? commandLineArgs = null, string language = "c++", string languageStandard = DefaultCppStandard, IReadOnlyDictionary<string, string>? remappedTypeNames = null, IReadOnlyDictionary<string, string>? remappedFieldNames = null, string? withConditional = null, IReadOnlyDictionary<string, Guid>? withGuids = null)
        => ValidateCoreAsync(BaselineHarness.CaseName(method, discriminator), inputContents, additionalConfigOptions, excludedNames, remappedNames, withAccessSpecifiers, withAttributes, withCallConvs, withClasses, withLibraryPaths, withNamespaces, withSetLastErrors, withTransparentStructs, withTypes, withUsings, withPackings, expectedDiagnostics, libraryPath, commandLineArgs, language, languageStandard, remappedTypeNames, remappedFieldNames, withConditional, withGuids);

    private async Task ValidateCoreAsync(string caseName, string inputContents, PInvokeGeneratorConfigurationOptions additionalConfigOptions, string[]? excludedNames, IReadOnlyDictionary<string, string>? remappedNames, IReadOnlyDictionary<string, AccessSpecifier>? withAccessSpecifiers, IReadOnlyDictionary<string, IReadOnlyList<string>>? withAttributes, IReadOnlyDictionary<string, string>? withCallConvs, IReadOnlyDictionary<string, string>? withClasses, IReadOnlyDictionary<string, string>? withLibraryPaths, IReadOnlyDictionary<string, string>? withNamespaces, string[]? withSetLastErrors, IReadOnlyDictionary<string, (string, PInvokeGeneratorTransparentStructKind)>? withTransparentStructs, IReadOnlyDictionary<string, string>? withTypes, IReadOnlyDictionary<string, IReadOnlyList<string>>? withUsings, IReadOnlyDictionary<string, string>? withPackings, IEnumerable<Diagnostic>? expectedDiagnostics, string libraryPath, string[]? commandLineArgs, string language, string languageStandard, IReadOnlyDictionary<string, string>? remappedTypeNames, IReadOnlyDictionary<string, string>? remappedFieldNames, string? withConditional, IReadOnlyDictionary<string, Guid>? withGuids)
    {
        var effectiveCommandLineArgs = BaselineHarness.WithUnixTarget(commandLineArgs, DefaultCppClangCommandLineArgs, _variant.Os);
        var actual = await GenerateBindingsAsync(inputContents, _variant.Mode, _variant.ConfigOptions | additionalConfigOptions, excludedNames, remappedNames, withAccessSpecifiers, withAttributes, withCallConvs, withClasses, withLibraryPaths, withNamespaces, withSetLastErrors, withTransparentStructs, withTypes, withUsings, withPackings, expectedDiagnostics, libraryPath, effectiveCommandLineArgs, language, languageStandard, remappedTypeNames, remappedFieldNames, withConditional: withConditional, withGuids: withGuids).ConfigureAwait(false);
        await BaselineAssertions.AssertOrUpdateAsync(Area, caseName, _variant, actual).ConfigureAwait(false);
    }
}
