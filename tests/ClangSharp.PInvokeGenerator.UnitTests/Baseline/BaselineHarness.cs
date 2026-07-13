// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ClangSharp.UnitTests.Baseline;

// Baseline harness (Approach 2): checked-in baseline files resolved with a
// {Case}.{Mode}.{Config}.{OS} -> {Case}.{Mode}.{Config} -> {Case}.{Mode} fallback chain, so output that
// is identical across variants is stored once. Baselines are read from the source tree during local runs
// and from a copy next to the test assembly on CI (see the csproj), and are (re)written to the source tree
// by the UPDATE_BASELINES workflow.

public enum BaselineConfig
{
    Compatible,
    Default,
    Latest,
    Preview,
}

public static class BaselineConfigExtensions
{
    // The config-only configuration flags (no GenerateUnixTypes), shared by BaselineVariant.ConfigOptions and
    // the host-keyed standalone cases that must not fold in the OS flag.
    public static PInvokeGeneratorConfigurationOptions ToConfigOptions(this BaselineConfig config) => config switch {
        BaselineConfig.Compatible => PInvokeGeneratorConfigurationOptions.GenerateCompatibleCode,
        BaselineConfig.Latest => PInvokeGeneratorConfigurationOptions.GenerateLatestCode,
        BaselineConfig.Preview => PInvokeGeneratorConfigurationOptions.GeneratePreviewCode,
        _ => PInvokeGeneratorConfigurationOptions.None,
    };
}

public enum BaselineOs
{
    Windows,
    Unix,
}

public readonly record struct BaselineVariant(PInvokeGeneratorOutputMode Mode, BaselineConfig Config, BaselineOs Os)
{
    public string ModeToken => Mode == PInvokeGeneratorOutputMode.Xml ? "Xml" : "CSharp";

    public override string ToString() => $"{ModeToken}{Config}{Os}";

    // Maps a variant onto the same (outputMode, configOptions) the legacy ValidateGenerated{Config}{OS}
    // wrappers used, so the generated output is identical to the pre-migration harness.
    public PInvokeGeneratorConfigurationOptions ConfigOptions
    {
        get
        {
            var options = Config.ToConfigOptions();

            if (Os == BaselineOs.Unix)
            {
                options |= PInvokeGeneratorConfigurationOptions.GenerateUnixTypes;
            }

            return options;
        }
    }

    public bool MatchesHost
    {
        get
        {
            var hostIsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            return Os == BaselineOs.Windows ? hostIsWindows : !hostIsWindows;
        }
    }
}

public static class BaselineHarness
{
    public static readonly IReadOnlyList<BaselineVariant> AllVariants = BuildAllVariants();

    private static List<BaselineVariant> BuildAllVariants()
    {
        var variants = new List<BaselineVariant>(16);

        foreach (var mode in new[] { PInvokeGeneratorOutputMode.CSharp, PInvokeGeneratorOutputMode.Xml })
        {
            foreach (var config in Enum.GetValues<BaselineConfig>())
            {
                foreach (var os in Enum.GetValues<BaselineOs>())
                {
                    variants.Add(new BaselineVariant(mode, config, os));
                }
            }
        }

        return variants;
    }

    // Baselines are resolved from two roots so a test run works both locally and on CI. The source tree
    // (located via [CallerFilePath]) is where the baselines live and are (re)written by UPDATE_BASELINES,
    // and is preferred so a fresh update is picked up without a rebuild. CI builds are deterministic, so
    // [CallerFilePath] is remapped to a non-existent path there; the copy placed next to the test assembly
    // (wired up in the csproj) is the fallback that resolves on CI, mirroring how ClangUnsavedFile.h is
    // copied to the output directory.
    public static string SourceBaselinesRoot([CallerFilePath] string thisFilePath = "")
        => Path.Combine(Path.GetDirectoryName(thisFilePath)!, "Baselines");

    private static string OutputBaselinesRoot
        => Path.Combine(AppContext.BaseDirectory, "Baseline", "Baselines");

    private static IEnumerable<string> BaselinesRoots()
    {
        yield return SourceBaselinesRoot();
        yield return OutputBaselinesRoot;
    }

    public static bool ShouldUpdateBaselines
        => Environment.GetEnvironmentVariable("UPDATE_BASELINES") is "1" or "true";

    // The Clang target triple pinned for Unix variants. Without an explicit --target, clang parses with the
    // host's default triple, so target-dependent output (C++ name mangling, type/pointer/long sizes, record
    // layout) would be baked to whichever host generated the baseline. Pinning a fixed Unix triple makes the
    // Unix baseline deterministic across every CI host (Linux and macOS produce identical output), so a single
    // checked-in baseline is valid everywhere. Windows variants are only ever run on a Windows host (see the
    // MatchesHost gating), so their host default is already the Windows triple and needs no pin.
    public const string UnixTargetTriple = "x86_64-unknown-linux-gnu";

    // When true, the MatchesHost gating is bypassed so every variant runs on the current host. Used to (re)write
    // Unix baselines from a Windows box under UPDATE_BASELINES, and to validate them locally via RUN_ALL_VARIANTS,
    // both of which rely on the pinned Unix triple to reproduce the Unix host's output deterministically.
    public static bool RunAllVariants
        => ShouldUpdateBaselines || Environment.GetEnvironmentVariable("RUN_ALL_VARIANTS") is "1" or "true";

    // Appends the pinned Unix target triple for Unix variants, leaving Windows variants exactly as the caller
    // specified (null flows through to the C++ default inside GenerateBindingsAsync). The base args mirror that
    // same default so an explicit --target can be appended without otherwise changing the command line.
    public static string[]? WithUnixTarget(string[]? commandLineArgs, string[] defaultCommandLineArgs, BaselineOs os)
    {
        if (os != BaselineOs.Unix)
        {
            return commandLineArgs;
        }

        var baseArgs = commandLineArgs ?? defaultCommandLineArgs;
        return [.. baseArgs, $"--target={UnixTargetTriple}"];
    }

    private static string Extension(PInvokeGeneratorOutputMode mode)
        => mode == PInvokeGeneratorOutputMode.Xml ? "xml" : "cs";

    // Candidate baseline file names, most-specific first, matching the documented fallback chain.
    private static IEnumerable<string> CandidateNames(string caseName, BaselineVariant variant)
    {
        var ext = Extension(variant.Mode);
        var mode = variant.ModeToken;

        yield return $"{caseName}.{mode}.{variant.Config}.{variant.Os}.{ext}";
        yield return $"{caseName}.{mode}.{variant.Config}.{ext}";
        yield return $"{caseName}.{mode}.{ext}";
    }

    // Resolves the baseline that applies to a variant via the fallback chain; returns null when none exists.
    public static string? ResolveBaselinePath(string area, string caseName, BaselineVariant variant)
    {
        foreach (var root in BaselinesRoots())
        {
            foreach (var name in CandidateNames(caseName, variant))
            {
                var candidate = Path.Combine(root, area, name);

                if (File.Exists(candidate))
                {
                    return candidate;
                }
            }
        }

        return null;
    }

    // The most-specific path, used when writing a fresh baseline before de-duplication collapses it. Writes
    // always target the source tree so UPDATE_BASELINES updates the checked-in files, not the output copy.
    public static string MostSpecificPath(string area, string caseName, BaselineVariant variant)
    {
        using var e = CandidateNames(caseName, variant).GetEnumerator();
        _ = e.MoveNext();
        return Path.Combine(SourceBaselinesRoot(), area, e.Current);
    }

    // Filesystem-safe case token derived from the test method name plus any per-case discriminator.
    // The method name is always a valid C# identifier (already filesystem-safe); the discriminator can be
    // an arbitrary [TestCase] argument (operators like "%"/"|", pointer types like "int *"), so it is
    // encoded injectively: letters/digits pass through, every other character becomes `_` + its hex code
    // point. This is collision-free -- distinct discriminators always yield distinct tokens -- which the
    // naive "replace every non-alphanumeric with `_`" scheme was not (e.g. `%`, `|`, `^`, `*` all collapsed
    // to the same file, so per-operator baselines overwrote one another).
    public static string CaseName(string method, string? discriminator = null)
    {
        ArgumentNullException.ThrowIfNull(method);

        return discriminator is null ? method : $"{method}_{Encode(discriminator)}";
    }

    private static string Encode(string discriminator)
    {
        var sb = new System.Text.StringBuilder(discriminator.Length);

        foreach (var c in discriminator)
        {
            if (char.IsAsciiLetterOrDigit(c))
            {
                _ = sb.Append(c);
            }
            else if (c <= 0xFF)
            {
                _ = sb.Append('_').Append(((int)c).ToString("X2", CultureInfo.InvariantCulture));
            }
            else
            {
                _ = sb.Append("_u").Append(((int)c).ToString("X4", CultureInfo.InvariantCulture));
            }
        }

        return sb.ToString();
    }
}
