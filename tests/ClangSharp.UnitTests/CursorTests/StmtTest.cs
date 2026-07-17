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

    [Test]
    public void GCCAsmStmtTest()
    {
        var inputContents = """
void f(int x)
{
    int y;
    asm volatile ("mov %1, %0" : "=r"(y) : "r"(x) : "memory");
}
""";

        using var translationUnit = CreateTranslationUnit(inputContents);

        var func = translationUnit.TranslationUnitDecl.Decls.OfType<FunctionDecl>()
                                  .Single((functionDecl) => functionDecl.Name.Equals("f", StringComparison.Ordinal));
        var asmStmt = func.CursorChildren.OfType<CompoundStmt>().Single()
                          .Children.OfType<GCCAsmStmt>().Single();

        Assert.That(asmStmt.IsSimple, Is.False);
        Assert.That(asmStmt.IsVolatile, Is.True);
        Assert.That(asmStmt.IsAsmGoto, Is.False);

        Assert.That(asmStmt.NumOutputs, Is.EqualTo(1u));
        Assert.That(asmStmt.OutputConstraints, Has.Count.EqualTo(1));
        Assert.That(asmStmt.OutputConstraints[0], Is.EqualTo("=r"));
        Assert.That(asmStmt.OutputExprs, Has.Count.EqualTo(1));

        Assert.That(asmStmt.NumInputs, Is.EqualTo(1u));
        Assert.That(asmStmt.InputConstraints, Has.Count.EqualTo(1));
        Assert.That(asmStmt.InputConstraints[0], Is.EqualTo("r"));
        Assert.That(asmStmt.InputExprs, Has.Count.EqualTo(1));

        Assert.That(asmStmt.NumClobbers, Is.EqualTo(1u));
        Assert.That(asmStmt.Clobbers, Has.Count.EqualTo(1));
        Assert.That(asmStmt.Clobbers[0], Is.EqualTo("memory"));

        Assert.That(asmStmt.NumLabels, Is.EqualTo(0u));
        Assert.That(asmStmt.AsmString, Is.Not.Empty);
    }

    [Test]
    public void GCCAsmGotoTest()
    {
        var inputContents = """
void f(int x)
{
    asm goto ("" : : "r"(x) : : lbl);
lbl:
    ;
}
""";

        using var translationUnit = CreateTranslationUnit(inputContents);

        var func = translationUnit.TranslationUnitDecl.Decls.OfType<FunctionDecl>()
                                  .Single((functionDecl) => functionDecl.Name.Equals("f", StringComparison.Ordinal));
        var asmStmt = func.CursorChildren.OfType<CompoundStmt>().Single()
                          .Children.OfType<GCCAsmStmt>().Single();

        Assert.That(asmStmt.IsAsmGoto, Is.True);
        Assert.That(asmStmt.NumLabels, Is.EqualTo(1u));
        Assert.That(asmStmt.LabelNames, Has.Count.EqualTo(1));
        Assert.That(asmStmt.LabelNames[0], Is.EqualTo("lbl"));
        Assert.That(asmStmt.LabelExprs, Has.Count.EqualTo(1));
    }
}
