// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Threading.Tasks;
using ClangSharp.UnitTests.Baseline;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

/// <summary>
/// Regression tests for https://github.com/dotnet/ClangSharp/issues/504.
/// The opt-in <c>generate-extern-variables</c> option surfaces top-level <c>extern</c> /
/// <c>extern const</c> global variables (which the generator otherwise drops, since there is nothing to
/// constant-fold). Mirroring <c>--with-manual-import</c>, each global is emitted as a settable instance
/// pointer field on a synthesized <c>&lt;Class&gt;ManualImports</c> struct that the consumer populates
/// itself, so the generator owns no <see cref="System.Runtime.InteropServices.NativeLibrary"/> handle.
/// This is strictly opt-in because it emits API surface and only supports pointer and primitive shapes;
/// const-ness is carried by <c>[NativeTypeName]</c>. Unsupported shapes (record-by-value, arrays) are
/// skipped with a diagnostic, and the default output is unchanged when the option is off.
/// </summary>
[Platform("win")]
public sealed class ExternVariableTest : StandaloneBaselineTest
{
    protected override string Area => "ExternVariable";

    [Test]
    public Task ExternGlobalsAreBound()
    {
        var inputContents = @"extern const int kMyConstInt;
extern int MyMutableInt;
extern void* MyMutablePointer;
";

        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(inputContents, additionalConfigOptions: PInvokeGeneratorConfigurationOptions.GenerateExternVariables);
    }

    [Test]
    public Task UnsupportedExternTypesAreSkipped()
    {
        var inputContents = @"struct MyStruct
{
    int X;
};

extern MyStruct MyGlobalStruct;
";

        var diagnostics = new[] {
            new Diagnostic(DiagnosticLevel.Warning, "Extern variable 'MyGlobalStruct' has an unsupported type 'MyStruct' for 'generate-extern-variables' and was skipped. Only pointer and primitive types are supported.", "Line 6, Column 17 in ClangUnsavedFile.h")
        };

        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(inputContents, additionalConfigOptions: PInvokeGeneratorConfigurationOptions.GenerateExternVariables, expectedDiagnostics: diagnostics);
    }

    [Test]
    public Task ExternGlobalsAreLeftUntouchedWhenNotOptedIn()
    {
        var inputContents = @"extern const int kMyConstInt;
extern int MyMutableInt;
";

        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(inputContents);
    }
}
