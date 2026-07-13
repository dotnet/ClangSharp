// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

/// <summary>
/// Regression test for https://github.com/dotnet/ClangSharp/issues/555.
/// When <c>generate-file-scoped-namespaces</c> is used with a single output file, no namespace
/// opening brace is emitted, so no closing brace must be emitted either.
/// </summary>
[Platform("win")]
public sealed class FileScopedNamespaceTest : PInvokeGeneratorTest
{
    [Test]
    public Task MethodClassHasNoTrailingBrace()
    {
        var inputContents = @"extern ""C"" void MyFunction();
";

        var expectedOutputContents = @"using System.Runtime.InteropServices;

namespace ClangSharp.Test;

public static partial class Methods
{
    [DllImport(""ClangSharpPInvokeGenerator"", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern void MyFunction();
}
";

        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorConfigurationOptions.GenerateFileScopedNamespaces);
    }

    [Test]
    public Task StructHasNoTrailingBrace()
    {
        var inputContents = @"struct Point
{
    int x;
    int y;
};
";

        var expectedOutputContents = @"namespace ClangSharp.Test;

public partial struct Point
{
    public int x;

    public int y;
}
";

        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorConfigurationOptions.GenerateFileScopedNamespaces);
    }
}
