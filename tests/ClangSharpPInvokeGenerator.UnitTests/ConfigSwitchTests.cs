// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using NUnit.Framework;
using static ClangSharp.PInvokeGeneratorConfigurationOptions;

namespace ClangSharp.UnitTests;

public sealed class ConfigSwitchTests
{
    private static PInvokeGeneratorConfigurationOptions ApplyOk(string configSwitch, string value, PInvokeGeneratorConfigurationOptions start = None)
    {
        var errorList = new List<string>();
        var warningList = new List<string>();

        var result = Program.ApplyConfigSwitch(configSwitch, value, start, errorList, warningList);

        Assert.That(errorList, Is.Empty);
        Assert.That(warningList, Is.Empty);
        return result;
    }

    [Test]
    public void PositiveBooleanSwitchSetsAndClears()
    {
        Assert.That(ApplyOk("generate-aggressive-inlining", "") & GenerateAggressiveInlining, Is.EqualTo(GenerateAggressiveInlining));
        Assert.That(ApplyOk("generate-aggressive-inlining", "true") & GenerateAggressiveInlining, Is.EqualTo(GenerateAggressiveInlining));
        Assert.That(ApplyOk("generate-aggressive-inlining", "false", GenerateAggressiveInlining) & GenerateAggressiveInlining, Is.EqualTo(None));
    }

    [Test]
    public void InvertedBooleanSwitchOptsOutWithFalse()
    {
        // The feature is on by default; `=false` records the opt-out flag, `=true` clears it.
        Assert.That(ApplyOk("generate-com-proxies", "false") & ExcludeComProxies, Is.EqualTo(ExcludeComProxies));
        Assert.That(ApplyOk("generate-com-proxies", "true", ExcludeComProxies) & ExcludeComProxies, Is.EqualTo(None));
        Assert.That(ApplyOk("generate-com-proxies", "") & ExcludeComProxies, Is.EqualTo(None));
    }

    [Test]
    public void CodegenFamilyIsMutuallyExclusive()
    {
        var preview = ApplyOk("codegen", "preview", GenerateCompatibleCode);
        Assert.That(preview & GeneratePreviewCode, Is.EqualTo(GeneratePreviewCode));
        Assert.That(preview & (GenerateCompatibleCode | GenerateLatestCode), Is.EqualTo(None));

        var compatible = ApplyOk("codegen", "compatible", GeneratePreviewCode);
        Assert.That(compatible & GenerateCompatibleCode, Is.EqualTo(GenerateCompatibleCode));
        Assert.That(compatible & (GenerateLatestCode | GeneratePreviewCode), Is.EqualTo(None));

        Assert.That(ApplyOk("codegen", "default", GeneratePreviewCode) & (GenerateCompatibleCode | GenerateLatestCode | GeneratePreviewCode), Is.EqualTo(None));
    }

    [Test]
    public void VtblFamilyIsMutuallyExclusive()
    {
        var trimmable = ApplyOk("vtbls", "trimmable", GenerateExplicitVtbls);
        Assert.That(trimmable & GenerateTrimmableVtbls, Is.EqualTo(GenerateTrimmableVtbls));
        Assert.That(trimmable & GenerateExplicitVtbls, Is.EqualTo(None));

        var explicitVtbls = ApplyOk("vtbls", "explicit", GenerateTrimmableVtbls);
        Assert.That(explicitVtbls & GenerateExplicitVtbls, Is.EqualTo(GenerateExplicitVtbls));
        Assert.That(explicitVtbls & GenerateTrimmableVtbls, Is.EqualTo(None));

        Assert.That(ApplyOk("vtbls", "implicit", GenerateExplicitVtbls | GenerateTrimmableVtbls) & (GenerateExplicitVtbls | GenerateTrimmableVtbls), Is.EqualTo(None));
    }

    [Test]
    public void FileAndTypesFamiliesToggleTheirFlag()
    {
        Assert.That(ApplyOk("file", "multi") & GenerateMultipleFiles, Is.EqualTo(GenerateMultipleFiles));
        Assert.That(ApplyOk("file", "single", GenerateMultipleFiles) & GenerateMultipleFiles, Is.EqualTo(None));

        Assert.That(ApplyOk("types", "unix") & GenerateUnixTypes, Is.EqualTo(GenerateUnixTypes));
        Assert.That(ApplyOk("types", "windows", GenerateUnixTypes) & GenerateUnixTypes, Is.EqualTo(None));
    }

    [Test]
    public void DeprecatedSwitchWarnsAndMapsToCanonical()
    {
        var errorList = new List<string>();
        var warningList = new List<string>();

        var result = Program.ApplyConfigSwitch("exclude-com-proxies", "", None, errorList, warningList);

        Assert.That(errorList, Is.Empty);
        Assert.That(result & ExcludeComProxies, Is.EqualTo(ExcludeComProxies));
        Assert.That(warningList, Has.Count.EqualTo(1));
        Assert.That(warningList[0], Does.Contain("exclude-com-proxies"));
        Assert.That(warningList[0], Does.Contain("generate-com-proxies=false"));
    }

    [Test]
    public void DeprecatedFamilySwitchMapsToValuedCanonical()
    {
        var errorList = new List<string>();
        var warningList = new List<string>();

        var result = Program.ApplyConfigSwitch("multi-file", "", None, errorList, warningList);

        Assert.That(errorList, Is.Empty);
        Assert.That(result & GenerateMultipleFiles, Is.EqualTo(GenerateMultipleFiles));
        Assert.That(warningList[0], Does.Contain("file=multi"));
    }

    [Test]
    public void UnrecognizedSwitchProducesAnError()
    {
        var errorList = new List<string>();
        var warningList = new List<string>();

        _ = Program.ApplyConfigSwitch("not-a-real-switch", "", None, errorList, warningList);

        Assert.That(errorList, Has.Count.EqualTo(1));
        Assert.That(errorList[0], Does.Contain("not-a-real-switch"));
    }

    [Test]
    public void InvalidBooleanValueProducesAnError()
    {
        var errorList = new List<string>();
        var warningList = new List<string>();

        _ = Program.ApplyConfigSwitch("generate-guid-member", "maybe", None, errorList, warningList);

        Assert.That(errorList, Has.Count.EqualTo(1));
        Assert.That(errorList[0], Does.Contain("maybe"));
    }

    [Test]
    public void InvalidFamilyValueProducesAnError()
    {
        var errorList = new List<string>();
        var warningList = new List<string>();

        _ = Program.ApplyConfigSwitch("codegen", "bogus", None, errorList, warningList);

        Assert.That(errorList, Has.Count.EqualTo(1));
        Assert.That(errorList[0], Does.Contain("bogus"));
    }

    [Test]
    public void GenerateTestsSwitchRequiresTestOutputLocation()
    {
        var errorList = new List<string>();
        var warningList = new List<string>();

        // Without a test output location the switch records an error; with one it just sets the flag.
        var missing = Program.ApplyFullConfigSwitch("generate-tests-nunit", "", None, testOutputLocation: null, errorList, warningList);
        Assert.That(missing & GenerateTestsNUnit, Is.EqualTo(GenerateTestsNUnit));
        Assert.That(errorList, Has.Count.EqualTo(1));
        Assert.That(errorList[0], Does.Contain("test output"));

        errorList.Clear();
        var present = Program.ApplyFullConfigSwitch("generate-tests-nunit", "", None, testOutputLocation: "out.cs", errorList, warningList);
        Assert.That(present & GenerateTestsNUnit, Is.EqualTo(GenerateTestsNUnit));
        Assert.That(errorList, Is.Empty);
        Assert.That(warningList, Is.Empty);
    }

    [Test]
    public void GenerateTestsSwitchesAreMutuallyExclusive()
    {
        var errorList = new List<string>();
        var warningList = new List<string>();

        _ = Program.ApplyFullConfigSwitch("generate-tests-xunit", "", GenerateTestsNUnit, testOutputLocation: "out.cs", errorList, warningList);

        Assert.That(errorList, Has.Count.EqualTo(1));
        Assert.That(errorList[0], Does.Contain("both NUnit and XUnit"));
    }

    [Test]
    public void GenerateTestsSwitchClearsWithFalse()
    {
        var errorList = new List<string>();
        var warningList = new List<string>();

        var result = Program.ApplyFullConfigSwitch("generate-tests-nunit", "false", GenerateTestsNUnit, testOutputLocation: null, errorList, warningList);

        Assert.That(result & GenerateTestsNUnit, Is.EqualTo(None));
        Assert.That(errorList, Is.Empty);
        Assert.That(warningList, Is.Empty);
    }
}
