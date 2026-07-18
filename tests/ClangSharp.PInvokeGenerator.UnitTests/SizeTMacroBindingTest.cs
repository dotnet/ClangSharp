// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Threading.Tasks;
using ClangSharp.UnitTests.Baseline;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

/// <summary>Provides validation that a macro whose value is a <c>size_t</c> expression (clang 22 models this as a
/// <c>PredefinedSugarType</c> spelled <c>__size_t</c>) resolves to its underlying primitive rather than leaking the
/// internal type name or emitting a malformed <c>ref readonly</c> getter under <c>unmanaged-constants</c>.</summary>
[Platform("win")]
public sealed class SizeTMacroBindingTest : StandaloneBaselineTest
{
    protected override string Area => "SizeTMacroBinding";

    [Test]
    public Task SizeTMacroTest()
    {
        var inputContents = @"#define MY_INT_SIZE sizeof(int)";

        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(inputContents);
    }

    [Test]
    public Task SizeTGenerateUnmanagedConstantsMacroTest()
    {
        var inputContents = @"#define MY_INT_SIZE sizeof(int)";

        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(inputContents, PInvokeGeneratorConfigurationOptions.GenerateUnmanagedConstants);
    }
}
