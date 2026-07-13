// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

/// <summary>
/// Regression test for https://github.com/dotnet/ClangSharp/issues/606.
/// A named nested record whose name matches a sibling field would emit a nested type and a field
/// of the same name in the same parent, which is a CS0102 clash in C#. The type is auto-renamed
/// (with a warning) and can be controlled explicitly via <c>--remap-type</c> / <c>--remap-field</c>.
/// </summary>
[Platform("win")]
public sealed class UnionFieldTypeNameClashTest : PInvokeGeneratorTest
{
    private const string InputContents = @"union GpuCaptureUnion
{
    struct GpuCaptureParameters
    {
        int a;
        int b;
    } GpuCaptureParameters;
    int other;
};
";

    [Test]
    public Task AutoRenamesClashingTypeAndWarns()
    {
        var expectedOutputContents = @"using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct GpuCaptureUnion
    {
        [FieldOffset(0)]
        [NativeTypeName(""struct GpuCaptureParameters"")]
        public _GpuCaptureParameters_e__Struct GpuCaptureParameters;

        [FieldOffset(0)]
        public int other;

        public partial struct _GpuCaptureParameters_e__Struct
        {
            public int a;

            public int b;
        }
    }
}
";

        var expectedDiagnostics = new[] {
            new Diagnostic(DiagnosticLevel.Warning, "Renamed nested type 'GpuCaptureParameters' to '_GpuCaptureParameters_e__Struct' to avoid a name clash with a field of the same name in 'GpuCaptureUnion'. Use '--remap-type GpuCaptureParameters=NewName' or '--remap-field GpuCaptureParameters=NewName' to control the naming explicitly.", "Line 3, Column 12 in ClangUnsavedFile.h")
        };

        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(InputContents, expectedOutputContents, expectedDiagnostics: expectedDiagnostics, commandLineArgs: DefaultCClangCommandLineArgs, language: "c", languageStandard: DefaultCStandard);
    }

    [Test]
    public Task RemapTypeControlsClashingTypeName()
    {
        var expectedOutputContents = @"using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct GpuCaptureUnion
    {
        [FieldOffset(0)]
        [NativeTypeName(""struct GpuCaptureParameters"")]
        public GpuCaptureParametersData GpuCaptureParameters;

        [FieldOffset(0)]
        public int other;

        public partial struct GpuCaptureParametersData
        {
            public int a;

            public int b;
        }
    }
}
";

        var remappedTypeNames = new Dictionary<string, string> {
            ["GpuCaptureParameters"] = "GpuCaptureParametersData"
        };

        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(InputContents, expectedOutputContents, commandLineArgs: DefaultCClangCommandLineArgs, language: "c", languageStandard: DefaultCStandard, remappedTypeNames: remappedTypeNames);
    }

    [Test]
    public Task RemapFieldControlsClashingFieldName()
    {
        var expectedOutputContents = @"using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct GpuCaptureUnion
    {
        [FieldOffset(0)]
        [NativeTypeName(""struct GpuCaptureParameters"")]
        public GpuCaptureParameters GpuCaptureParametersField;

        [FieldOffset(0)]
        public int other;

        public partial struct GpuCaptureParameters
        {
            public int a;

            public int b;
        }
    }
}
";

        var remappedFieldNames = new Dictionary<string, string> {
            ["GpuCaptureParameters"] = "GpuCaptureParametersField"
        };

        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(InputContents, expectedOutputContents, commandLineArgs: DefaultCClangCommandLineArgs, language: "c", languageStandard: DefaultCStandard, remappedFieldNames: remappedFieldNames);
    }
}
