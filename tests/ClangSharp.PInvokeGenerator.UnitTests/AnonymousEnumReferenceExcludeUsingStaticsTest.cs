// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

/// <summary>
/// Regression test for https://github.com/dotnet/ClangSharp/issues/681.
/// When <c>exclude-using-statics-for-enums</c> is used, a reference to an anonymous enum member
/// must be qualified with the backing class the constants are emitted on (e.g. <c>Methods</c>)
/// rather than the non-existent <c>__AnonymousEnum_*</c> enum name.
/// </summary>
[Platform("win")]
public sealed class AnonymousEnumReferenceExcludeUsingStaticsTest : PInvokeGeneratorTest
{
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

        var expectedOutputContents = @"namespace ClangSharp.Test
{
    public enum MyEnum2
    {
        MyEnum2_Value1 = Methods.MyEnum1_Value1,
    }

    public static partial class Methods
    {
        public const int MyEnum1_Value1 = 1;
    }
}
";

        var diagnostics = new[] { new Diagnostic(DiagnosticLevel.Info, "Found anonymous enum: __AnonymousEnum_ClangUnsavedFile_L1_C1. Mapping values as constants in: Methods", "Line 1, Column 1 in ClangUnsavedFile.h") };
        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorConfigurationOptions.DontUseUsingStaticsForEnums, expectedDiagnostics: diagnostics);
    }
}
