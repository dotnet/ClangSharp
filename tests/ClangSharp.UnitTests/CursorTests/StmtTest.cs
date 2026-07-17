// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Linq;
using ClangSharp.Interop;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

public sealed class StmtTest : TranslationUnitTest
{
    [Test]
    public void IfStmtStatementKindTest()
    {
        var inputContents = """
void f(int x)
{
    if (x) { }
    if constexpr (sizeof(int) == 4) { }
}
""";

        string[] commandLineArgs = ["-std=c++20", "-Wno-pragma-once-outside-header"];
        using var translationUnit = CreateTranslationUnit(inputContents, commandLineArgs: commandLineArgs);

        var func = translationUnit.TranslationUnitDecl.Decls.OfType<FunctionDecl>()
                                  .Single((functionDecl) => functionDecl.Name.Equals("f", StringComparison.Ordinal));
        var ifStmts = func.CursorChildren.OfType<CompoundStmt>().Single()
                          .Children.OfType<IfStmt>()
                          .ToArray();

        Assert.That(ifStmts, Has.Length.EqualTo(2));

        Assert.That(ifStmts[0].StatementKind, Is.EqualTo(CX_IfStatementKind.CX_ISK_Ordinary));
        Assert.That(ifStmts[0].IsConstexpr, Is.False);

        Assert.That(ifStmts[1].StatementKind, Is.EqualTo(CX_IfStatementKind.CX_ISK_Constexpr));
        Assert.That(ifStmts[1].IsConstexpr, Is.True);
    }
}
