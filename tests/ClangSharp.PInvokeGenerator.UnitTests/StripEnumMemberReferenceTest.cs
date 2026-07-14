// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Threading.Tasks;
using ClangSharp.UnitTests.Baseline;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

/// <summary>
/// Regression test for https://github.com/dotnet/ClangSharp/issues/576.
/// When <c>strip-enum-member-type-name</c> is used, references to sibling enum members inside
/// an initializer expression must be stripped the same way as the member declarations, otherwise
/// the emitted code references undefined names.
/// </summary>
[Platform("win")]
public sealed class StripEnumMemberReferenceTest : StandaloneBaselineTest
{
    protected override string Area => "StripEnumMemberReference";

    [Test]
    public Task SiblingReferencesAreStripped()
    {
        var inputContents = @"typedef enum WGPUInstanceBackend
{
    WGPUInstanceBackend_Vulkan = 1 << 0,
    WGPUInstanceBackend_GL = 1 << 1,
    WGPUInstanceBackend_Metal = 1 << 2,
    WGPUInstanceBackend_Primary = WGPUInstanceBackend_Vulkan | WGPUInstanceBackend_Metal,
    WGPUInstanceBackend_Secondary = WGPUInstanceBackend_GL
} WGPUInstanceBackend;
";

        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(inputContents, PInvokeGeneratorConfigurationOptions.StripEnumMemberTypeName);
    }
}
