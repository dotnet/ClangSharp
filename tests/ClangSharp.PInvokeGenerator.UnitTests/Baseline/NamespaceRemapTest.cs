// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests.Baseline;

[TestFixtureSource(nameof(Variants))]
public sealed class NamespaceRemapTest : BaselineTest
{
    public NamespaceRemapTest(BaselineVariant variant) : base(variant)
    {
    }

    protected override string Area => "NamespaceRemap";

    // A `--remap` keyed by the fully qualified name of a namespaced C++ type (e.g. `Ns.Point`) must apply to
    // both the type declaration and every reference to it. The reference resolves through the same decl as the
    // declaration, so a use-site field must emit the remapped name rather than the unqualified leaf, keeping the
    // declaration and its references in agreement.
    [Test]
    public Task NamespaceQualifiedRemapAppliesAtUseSiteTest()
    {
        var inputContents = @"namespace Ns
{
    struct Point
    {
        int X;
        int Y;
    };

    enum Status
    {
        Err = -1,
        Ok = 0,
    };
}

struct Holder
{
    Ns::Point point;
    Ns::Point* pointPtr;
    Ns::Status status;
};
";

        var remappedNames = new Dictionary<string, string> {
            ["Ns.Point"] = "NsPoint",
            ["Ns.Status"] = "NsStatus",
        };

        return ValidateAsync(nameof(NamespaceQualifiedRemapAppliesAtUseSiteTest), inputContents, remappedNames: remappedNames);
    }
}
