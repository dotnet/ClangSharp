// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
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

    [Test]
    public void IsExternC()
    {
        AssertNeedNewClangSharp();

        var inputContents =
    $$"""
    extern "C" {
        int theUniverseAndEverything = 0;
        int hitchhike ();
    }
    """;

        using var translationUnit = CreateTranslationUnit(inputContents);

        var allDecls = new List<Decl>();
        allDecls.AddRange(translationUnit.TranslationUnitDecl.Decls);
        allDecls.AddRange(allDecls.OfType<LinkageSpecDecl>().SelectMany(v => v.Decls).ToList());

        var functionDecls = allDecls.OfType<FunctionDecl>().ToList();
        Assert.That(functionDecls.Count, Is.GreaterThan(0), "Function");
        foreach (var decl in functionDecls)
        {
            Assert.That(decl.IsExternC, Is.True, "IsExternC function");
        }

        var varDecls = allDecls.OfType<VarDecl>().ToList();
        Assert.That(varDecls.Count, Is.GreaterThan(0), "Var");
        foreach (var decl in varDecls)
        {
            Assert.That(decl.IsExternC, Is.True, "IsExternC var");
        }
    }
}
