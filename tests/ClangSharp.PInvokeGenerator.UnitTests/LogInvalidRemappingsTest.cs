// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

public sealed class LogInvalidRemappingsTest : PInvokeGeneratorTest
{
    [Test]
    public Task FixedSizeBufferRemappingIsLoggedAndLoweredToPointer()
    {
        var inputContents = @"typedef int MyBuffer;

extern ""C"" void MyFunction(MyBuffer value);
extern ""C"" MyBuffer MyOtherFunction();";

        var expectedOutputContents = @"using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public static unsafe partial class Methods
    {
        [DllImport(""ClangSharpPInvokeGenerator"", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void MyFunction([NativeTypeName(""MyBuffer"")] sbyte* value);

        [DllImport(""ClangSharpPInvokeGenerator"", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName(""MyBuffer"")]
        public static extern sbyte* MyOtherFunction();
    }
}
";

        var remappedNames = new Dictionary<string, string> {
            ["MyBuffer"] = "sbyte[8]"
        };

        var expectedDiagnostics = new[] {
            new Diagnostic(DiagnosticLevel.Info, "Potential invalid remapping 'MyBuffer=sbyte[8]'. A fixed-size-buffer type is not directly expressible in an unmanaged signature; it was lowered to the pointer 'sbyte*'.")
        };

        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents, additionalConfigOptions: PInvokeGeneratorConfigurationOptions.LogInvalidRemappings, remappedNames: remappedNames, expectedDiagnostics: expectedDiagnostics);
    }
}
