// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Threading.Tasks;
using ClangSharp.UnitTests.Baseline;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

/// <summary>
/// Regression test for https://github.com/dotnet/ClangSharp/issues/444.
/// Under <c>generate-cpp-attributes</c>, a C++ attribute (e.g. a SAL <c>_Field_size_full_</c>
/// annotation surfaced as an <c>annotate</c> attribute) on a struct field was not emitted as a
/// <c>[CppAttributeList("...")]</c>, even though the same attribute on a parameter was. Fields now
/// emit the attribute the same way parameters do.
/// </summary>
[Platform("win")]
public sealed class SalFieldAttributeTest : StandaloneBaselineTest
{
    protected override string Area => "SalFieldAttribute";

    private const string InputContents = @"#define _Field_size_full_(x) __attribute__((annotate(""_Field_size_full_("" #x "")"")))

struct MyStruct
{
    int count;
    _Field_size_full_(count) int* data;
};
";

    [Test]
    public Task FieldSalAnnotationGeneratesCppAttributeList()
    {
        var expectedDiagnostics = new[] {
            new Diagnostic(DiagnosticLevel.Warning, "Function like macro definition records are not supported: '_Field_size_full_'. Generated bindings may be incomplete.", "Line 1, Column 9 in ClangUnsavedFile.h")
        };

        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(InputContents, additionalConfigOptions: PInvokeGeneratorConfigurationOptions.GenerateCppAttributes, expectedDiagnostics: expectedDiagnostics);
    }
}
