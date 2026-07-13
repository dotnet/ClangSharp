// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Threading.Tasks;
using ClangSharp.UnitTests.Baseline;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

public sealed class BooleanBitfieldTest : StandaloneBaselineTest
{
    protected override string Area => "BooleanBitfield";

    // Regression test for https://github.com/dotnet/clangsharp/issues/508
    // A `bool` bitfield previously generated invalid C# (a `bool` backing field plus
    // `bool & int` arithmetic and `(bool)` casts). It should use an integer backing store
    // with the public accessor kept as `bool`, converting at the get/set boundary. An
    // adjacent non-bool bitfield must be unaffected.
    private const string InputContents = @"struct MyStruct
{
    bool a : 1;
    bool b : 1;
    int c : 2;
};
";

    [Test]
    public Task LatestTest()
    {
        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(InputContents);
    }

    [Test]
    public Task CompatibleTest()
    {
        return ValidateGeneratedCSharpCompatibleWindowsBaselineAsync(InputContents);
    }
}
