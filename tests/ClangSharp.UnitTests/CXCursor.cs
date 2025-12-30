// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Linq;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

public class CXCursorTest : TranslationUnitTest
{
    [Test]
    public void AttrKindSpelling()
    {

        var inputContents =
    $$"""
    int theUniverseAndEverything ();
    int main () { return 0; }
    """;

        using var translationUnit = CreateTranslationUnit(inputContents);

        var functionDecls = translationUnit.TranslationUnitDecl.Decls.OfType<FunctionDecl>().ToList ();
        Assert.That (functionDecls.Count, Is.GreaterThan (0), "Function");
        foreach (var functionDecl in functionDecls) {
            Assert.That (functionDecl.Handle.AttrKindSpelling, Is.Not.Null.Or.Empty, "AttrKindSpelling");
        }
    }
}
