// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

public sealed class ExcludeFunctionsTest : PInvokeGeneratorTest
{
    [Test]
    public Task ExcludesFreeFunctions()
    {
        var inputContents = @"extern ""C"" void MyFunction(int value);";

        var expectedOutputContents = string.Empty;

        return ValidateGeneratedCSharpDefaultWindowsBindingsAsync(inputContents, expectedOutputContents,
            additionalConfigOptions: PInvokeGeneratorConfigurationOptions.ExcludeFunctions);
    }

    [Test]
    public Task ExcludesMethodsButKeepsFields()
    {
        var inputContents = @"
class Box
{
public:
    float Width;
    float Height;
    float Area() const;
};
";

        var expectedOutputContents = @"namespace ClangSharp.Test
{
    public partial struct Box
    {
        public float Width;

        public float Height;
    }
}
";

        return ValidateGeneratedCSharpDefaultWindowsBindingsAsync(inputContents, expectedOutputContents,
            additionalConfigOptions: PInvokeGeneratorConfigurationOptions.ExcludeFunctions);
    }

    [Test]
    public Task ExcludesConstructors()
    {
        var inputContents = @"
class Box
{
public:
    float Width;
    Box();
    Box(float width);
};
";

        var expectedOutputContents = @"namespace ClangSharp.Test
{
    public partial struct Box
    {
        public float Width;
    }
}
";

        return ValidateGeneratedCSharpDefaultWindowsBindingsAsync(inputContents, expectedOutputContents,
            additionalConfigOptions: PInvokeGeneratorConfigurationOptions.ExcludeFunctions);
    }
}
