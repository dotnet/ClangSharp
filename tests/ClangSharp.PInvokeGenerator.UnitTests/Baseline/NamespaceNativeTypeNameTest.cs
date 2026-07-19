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
    // spelling for typedef, record (including by-pointer, const-qualified, and elaborated `struct`/`enum`
    // specifiers), and enum references, placing the qualifier after any leading cv-qualifiers or tag
    // keywords, and stays version-stable.
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
        const Point* constPointPtr;
        const Real* constRealPtr;
        struct Point* elabPointPtr;
        const struct Point* constElabPointPtr;
        enum Status* elabStatusPtr;
        Real r;
        Status status;
    };
}
";

        return ValidateAsync(nameof(NamespaceQualifiedNativeTypeNameIsPreservedTest), inputContents);
    }

    // A reference spelled from within a nested namespace already carries part of the enclosing
    // qualifier (e.g. `Windows::Foundation::PropertyValue` written inside `Abi`). The restored prefix
    // must only add the leading segments the spelling is missing, so it stays
    // `Abi::Windows::Foundation::PropertyValue` rather than doubling into
    // `Abi::Windows::Foundation::Windows::Foundation::PropertyValue`.
    [Test]
    public Task PartiallyQualifiedNamespaceIsNotDuplicatedTest()
    {
        var inputContents = @"namespace Abi
{
    namespace Windows
    {
        namespace Foundation
        {
            struct PropertyValue
            {
                int value;
            };
        }

        namespace Graphics
        {
            namespace Effects
            {
                struct EffectSource
                {
                    int source;
                };

                struct Interop
                {
                    Windows::Foundation::PropertyValue** partiallyQualified;
                    Abi::Windows::Foundation::PropertyValue** fullyQualified;
                    EffectSource** sameNamespace;
                };
            }
        }
    }
}
";

        return ValidateAsync(nameof(PartiallyQualifiedNamespaceIsNotDuplicatedTest), inputContents);
    }
}
