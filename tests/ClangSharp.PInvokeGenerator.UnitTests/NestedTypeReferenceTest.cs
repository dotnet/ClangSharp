// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Threading.Tasks;
using ClangSharp.UnitTests.Baseline;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

/// <summary>
/// Regression test for https://github.com/dotnet/ClangSharp/issues/579.
/// A nested type referenced from another scope must be qualified by its containing type(s)
/// (e.g. <c>A::Inner</c> -&gt; <c>A.Inner</c>) so it resolves in the generated C#.
/// </summary>
[Platform("win")]
public sealed class NestedTypeReferenceTest : StandaloneBaselineTest
{
    protected override string Area => "NestedTypeReference";

    [Test]
    public Task NestedTypeIsQualified()
    {
        var inputContents = @"struct A
{
    struct Inner
    {
        int value;
    };
};

struct B
{
    A::Inner inner;
};
";

        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(inputContents);
    }

    [Test]
    public Task NestedTypeIsNotQualifiedWithinContainer()
    {
        var inputContents = @"struct Outer
{
    struct Inner
    {
        int value;
    };

    Inner field;
    Inner* fieldPtr;
};
";

        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(inputContents);
    }
}
