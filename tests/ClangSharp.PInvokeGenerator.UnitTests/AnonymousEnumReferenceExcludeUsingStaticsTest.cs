// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Threading.Tasks;
using ClangSharp.UnitTests.Baseline;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

/// <summary>
/// Regression test for https://github.com/dotnet/ClangSharp/issues/681.
/// When <c>exclude-using-statics-for-enums</c> is used, a reference to an anonymous enum member
/// must be qualified with the backing class the constants are emitted on (e.g. <c>Methods</c>)
/// rather than the non-existent <c>__AnonymousEnum_*</c> enum name.
/// </summary>
[Platform("win")]
public sealed class AnonymousEnumReferenceExcludeUsingStaticsTest : StandaloneBaselineTest
{
    protected override string Area => "AnonymousEnumReferenceExcludeUsingStatics";

    [Test]
    public Task ReferenceIsQualifiedWithBackingClass()
    {
        var inputContents = @"enum
{
    MyEnum1_Value1 = 1,
};

enum MyEnum2 : int
{
    MyEnum2_Value1 = MyEnum1_Value1,
};
";

        var diagnostics = new[] { new Diagnostic(DiagnosticLevel.Info, "Found anonymous enum: __AnonymousEnum_ClangUnsavedFile_L1_C1. Mapping values as constants in: Methods", "Line 1, Column 1 in ClangUnsavedFile.h") };
        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(inputContents, PInvokeGeneratorConfigurationOptions.DontUseUsingStaticsForEnums, expectedDiagnostics: diagnostics);
    }
}
