// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Linq;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

public sealed class TypeTest : TranslationUnitTest
{
    [Test]
    public void UnqualifiedTypeTest()
    {
        // Regression test for dotnet/ClangSharp#562. A `Type` mirrors a Clang `QualType`, so the
        // local `const` cannot be stripped via sugar removal (`UnqualifiedDesugaredType` keeps it).
        // `UnqualifiedType` exposes `clang_getUnqualifiedType` to drop the local qualifiers.
        var inputContents = """
typedef int const* pcint;
pcint g();
""";

        using var translationUnit = CreateTranslationUnit(inputContents);

        var functionDecl = translationUnit.TranslationUnitDecl.Decls.OfType<FunctionDecl>().Single((functionDecl) => functionDecl.Name.Equals("g", StringComparison.Ordinal));
        var pointeeType = functionDecl.ReturnType.CanonicalType.PointeeType;

        Assert.That(pointeeType.IsLocalConstQualified, Is.True, "pointee should be const-qualified");
        Assert.That(pointeeType.UnqualifiedDesugaredType.IsLocalConstQualified, Is.True, "UnqualifiedDesugaredType keeps the local const");

        var unqualifiedType = pointeeType.UnqualifiedType;

        Assert.That(unqualifiedType.IsLocalConstQualified, Is.False, "UnqualifiedType should drop the local const");
        Assert.That(unqualifiedType.AsString, Is.EqualTo("int"));
    }
}
