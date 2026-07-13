// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Threading.Tasks;
using ClangSharp.UnitTests.Baseline;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

/// <summary>
/// Regression test for https://github.com/dotnet/ClangSharp/issues/694.
/// A deprecated message written as adjacent C string literals must be concatenated into a single
/// C# string literal, otherwise the emitted <c>[Obsolete(...)]</c> attribute is invalid C#.
/// </summary>
[Platform("win")]
public sealed class DeprecatedAdjacentStringTest : StandaloneBaselineTest
{
    protected override string Area => "DeprecatedAdjacentString";

    [Test]
    public Task AdjacentStringLiteralsAreConcatenated()
    {
        var inputContents = @"extern ""C"" [[deprecated(""Use Bar() or ""
                            ""Baz() instead"")]]
void MyFunction();
";

        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(inputContents);
    }
}
