// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

public sealed class OptionsTest()
    : PInvokeGeneratorTest(PInvokeGeneratorOutputMode.CSharp, PInvokeGeneratorConfigurationOptions.GenerateLatestCode)
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

        var withUsings = new Dictionary<string, IReadOnlyList<string>> {
            ["StructA"] = ["ForStructA1", "ForStructA2"],
            ["*"] = ["ForStar"],
            ["NS::StructB"] = ["ForStructBWithDoubleColon"],
            ["NS.StructC"] = ["ForStructCWithDot"],
            ["DoesNotExist"] = ["ErrorShouldNotBeInOutput"],
        };

        return ValidateGeneratedBindingsAsync(inputContents, withUsings: withUsings);
    }

    [Test]
    public Task WithAttributes()
    {
        var inputContents = @"struct StructA {}; struct StructB {}; struct StructC {}; struct StructD {};";

        var withAttributes = new Dictionary<string, IReadOnlyList<string>> {
            ["StructA"] = [@"A"],
            ["StructB"] = [@"B"],
            ["*"] = [@"Star"],
        };

        return ValidateGeneratedBindingsAsync(inputContents, withAttributes: withAttributes);
    }
}
