// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

public sealed class OptionsTest : PInvokeGeneratorTest
{
    [Test]
    public Task WithUsings()
    {
        var inputContents = @"struct StructA {};
namespace NS
{
    struct StructB {};
    struct StructC {};
}
struct StructD {};
";
        var expectedOutputContents = @"using ForStar;
using ForStructA1;
using ForStructA2;
using ForStructBWithDoubleColon;
using ForStructCWithDot;

namespace ClangSharp.Test
{
    public partial struct StructA
    {
    }

    public partial struct StructB
    {
    }

    public partial struct StructC
    {
    }

    public partial struct StructD
    {
    }
}
";
        var withUsings = new Dictionary<string, IReadOnlyList<string>> {
            ["StructA"] = ["ForStructA1", "ForStructA2"],
            ["*"] = ["ForStar"],
            ["NS::StructB"] = ["ForStructBWithDoubleColon"],
            ["NS.StructC"] = ["ForStructCWithDot"],
            ["DoesNotExist"] = ["ErrorShouldNotBeInOutput"],
        };

        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents, withUsings: withUsings);
    }

    [Test]
    public Task WithAttributes()
    {
        var inputContents = @"struct StructA {}; struct StructB {}; struct StructC {}; struct StructD {};";
        var expectedOutputContents =
@"namespace ClangSharp.Test
{
    [A]
    public partial struct StructA
    {
    }

    [B]
    public partial struct StructB
    {
    }

    [Star]
    public partial struct StructC
    {
    }

    [Star]
    public partial struct StructD
    {
    }
}
";
        var withAttributes = new Dictionary<string, IReadOnlyList<string>> {
            ["StructA"] = [@"A"],
            ["StructB"] = [@"B"],
            ["*"] = [@"Star"],
        };

        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents, withAttributes: withAttributes);
    }
}
