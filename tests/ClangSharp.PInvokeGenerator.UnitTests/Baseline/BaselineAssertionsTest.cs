// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using NUnit.Framework;

namespace ClangSharp.UnitTests.Baseline;

public sealed class BaselineAssertionsTest
{
    [Test]
    public void GeneratedCodeVersionIsResolvedAndNormalized()
    {
        const string baseline = "[assembly: GeneratedCode(\"ClangSharp\", \"{ClangSharpVersion}\")]";
        var actual = $"[assembly: GeneratedCode(\"ClangSharp\", \"{typeof(PInvokeGenerator).Assembly.GetName().Version}\")]";

        Assert.That(BaselineAssertions.ResolveGeneratedCodeVersion(baseline), Is.EqualTo(actual));
        Assert.That(BaselineAssertions.NormalizeGeneratedCodeVersion(actual), Is.EqualTo(baseline));
        Assert.That(BaselineAssertions.NormalizeGeneratedCodeVersion("GeneratedCode(\"ClangSharp\", \"wrong\")"), Is.EqualTo("GeneratedCode(\"ClangSharp\", \"wrong\")"));
        Assert.That("[assembly: GeneratedCode(\"ClangSharp\", \"wrong\")]", Is.Not.EqualTo(BaselineAssertions.ResolveGeneratedCodeVersion(baseline)));
    }
}
