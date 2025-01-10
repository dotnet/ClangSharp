// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.IO;
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

    [Test]
    public async Task TraversalTestsAsync()
    {
        var inputContents = "struct StructA {};";
        var expectedOutputContents =
@"namespace ClangSharp.Test
{
    public partial struct StructA
    {
    }
}
";

#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
        // Match everything.
        await ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents, withTraversalNames: ["**/*"]);

        // Non-matching, then matching pattern.
        await ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents, withTraversalNames: ["nomatch", "*.h"]);

        // Windows-style path separators.
        await ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents, withTraversalNames: ["**\\*"]);

        // Full path, with no wildcards.
        await ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents, withTraversalNames: [Path.GetFullPath(DefaultInputFileName)]);

        // Full path to parent, with file wildcard.
        await ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents, withTraversalNames: [Environment.CurrentDirectory + "/*"]);

        // Doesn't match
        await ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, string.Empty, withTraversalNames: ["nomatch", "*.cpp"]);
#pragma warning restore CA2007
    }
}
