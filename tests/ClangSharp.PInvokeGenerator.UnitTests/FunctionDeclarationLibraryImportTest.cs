// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

public sealed class FunctionDeclarationLibraryImportTest : PInvokeGeneratorTest
{
    [Test]
    public Task BasicTest()
    {
        var inputContents = @"extern ""C"" void MyFunction();";

        var expectedOutputContents = @"using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public static partial class Methods
    {
        [LibraryImport(""ClangSharpPInvokeGenerator"")]
        [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static partial void MyFunction();
    }
}
";

        return ValidateGeneratedCSharpDefaultWindowsBindingsAsync(inputContents, expectedOutputContents,
            additionalConfigOptions: PInvokeGeneratorConfigurationOptions.GenerateLibraryImport);
    }

    [Test]
    public Task WithParameterTest()
    {
        var inputContents = @"extern ""C"" void MyFunction(int value);";

        var expectedOutputContents = @"using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public static partial class Methods
    {
        [LibraryImport(""ClangSharpPInvokeGenerator"")]
        [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static partial void MyFunction(int value);
    }
}
";

        return ValidateGeneratedCSharpDefaultWindowsBindingsAsync(inputContents, expectedOutputContents,
            additionalConfigOptions: PInvokeGeneratorConfigurationOptions.GenerateLibraryImport);
    }

    [Test]
    public Task WithWinapiCallConvTest()
    {
        var inputContents = @"extern ""C"" void MyFunction();";

        var expectedOutputContents = @"using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public static partial class Methods
    {
        [LibraryImport(""ClangSharpPInvokeGenerator"")]
        public static partial void MyFunction();
    }
}
";

        var withCallConvs = new Dictionary<string, string>
        {
            ["*"] = "Winapi"
        };
        return ValidateGeneratedCSharpDefaultWindowsBindingsAsync(inputContents, expectedOutputContents,
            additionalConfigOptions: PInvokeGeneratorConfigurationOptions.GenerateLibraryImport,
            withCallConvs: withCallConvs);
    }

    [Test]
    public Task WithSetLastErrorTest()
    {
        var inputContents = @"extern ""C"" void MyFunction();";

        var expectedOutputContents = @"using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public static partial class Methods
    {
        [LibraryImport(""ClangSharpPInvokeGenerator"", SetLastError = true)]
        [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static partial void MyFunction();
    }
}
";

        return ValidateGeneratedCSharpDefaultWindowsBindingsAsync(inputContents, expectedOutputContents,
            additionalConfigOptions: PInvokeGeneratorConfigurationOptions.GenerateLibraryImport,
            withSetLastErrors: ["MyFunction"]);
    }

    [Test]
    public Task WithMixedCallConvsTest()
    {
        var inputContents = @"extern ""C"" void MyFunction1(int value); extern ""C"" void MyFunction2(int value);";

        var expectedOutputContents = @"using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public static partial class Methods
    {
        [LibraryImport(""ClangSharpPInvokeGenerator"")]
        public static partial void MyFunction1(int value);

        [LibraryImport(""ClangSharpPInvokeGenerator"")]
        [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static partial void MyFunction2(int value);
    }
}
";

        var withCallConvs = new Dictionary<string, string>
        {
            ["MyFunction1"] = "Winapi"
        };
        return ValidateGeneratedCSharpDefaultWindowsBindingsAsync(inputContents, expectedOutputContents,
            additionalConfigOptions: PInvokeGeneratorConfigurationOptions.GenerateLibraryImport,
            withCallConvs: withCallConvs);
    }
}
