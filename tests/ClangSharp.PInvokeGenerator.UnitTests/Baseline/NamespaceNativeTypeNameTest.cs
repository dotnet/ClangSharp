// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests.Baseline;

[TestFixtureSource(nameof(Variants))]
public sealed class NamespaceNativeTypeNameTest : BaselineTest
{
    public NamespaceNativeTypeNameTest(BaselineVariant variant) : base(variant)
    {
    }

    protected override string Area => "NamespaceNativeTypeName";

    // clang 22's type printer omits the enclosing C++ namespace from a reference spelled from within that
    // same namespace, where-as older releases always spelled it. The generator restores the dropped
    // `Namespace::` prefix from the decl so the emitted `NativeTypeName` keeps the fully qualified source
    // spelling for typedef, record (including by-pointer), and enum references and stays version-stable.
    [Test]
    public Task NamespaceQualifiedNativeTypeNameIsPreservedTest()
    {
        var inputContents = @"namespace Ns
{
    typedef float Real;

    struct Point
    {
        Real X;
        Real Y;
    };

    enum Status
    {
        Err = -1,
        Ok = 0,
    };

    struct Holder
    {
        Point point;
        Point* pointPtr;
        Real r;
        Status status;
    };
}
";

        return ValidateAsync(nameof(NamespaceQualifiedNativeTypeNameIsPreservedTest), inputContents);
    }
}
