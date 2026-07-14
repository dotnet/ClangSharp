// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests.Baseline;

[TestFixtureSource(nameof(Variants))]
public sealed class ConditionalOutputTest : BaselineTest
{
    public ConditionalOutputTest(BaselineVariant variant) : base(variant)
    {
    }

    protected override string Area => "ConditionalOutput";

    // --with-conditional wraps single-file C# output in a leading `#if <symbol>` and trailing `#endif`
    // (see dotnet/ClangSharp#365). XML output is unaffected, so the Xml baselines are identical to a run
    // without the option.
    [Test]
    public Task WrapsFileInConditional()
    {
        var inputContents = @"struct MyStruct
{
    int x;
};
";

        return ValidateAsync(nameof(WrapsFileInConditional), inputContents, withConditional: "VERSION_2_OR_NEWER");
    }
}
