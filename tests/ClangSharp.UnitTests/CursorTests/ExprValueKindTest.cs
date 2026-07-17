// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Linq;
using ClangSharp.Interop;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

public sealed class ExprValueKindTest : TranslationUnitTest
{
    [Test]
    public void ValueKindTest()
    {
        var inputContents = """
void f(int x)
{
    x;
    42;
    static_cast<int&&>(x);
}
""";

        string[] commandLineArgs = ["-std=c++20"];
        using var translationUnit = CreateTranslationUnit(inputContents, commandLineArgs: commandLineArgs);

        var func = translationUnit.TranslationUnitDecl.Decls.OfType<FunctionDecl>()
                                  .Single((functionDecl) => functionDecl.Name.Equals("f", StringComparison.Ordinal));
        var body = func.CursorChildren.OfType<CompoundStmt>().Single();

        var lvalue = body.Children.OfType<DeclRefExpr>().Single();
        Assert.That(lvalue.ValueKind, Is.EqualTo(CX_ExprValueKind.CX_VK_LValue));
        Assert.That(lvalue.IsLValue, Is.True);
        Assert.That(lvalue.IsGLValue, Is.True);
        Assert.That(lvalue.IsPRValue, Is.False);
        Assert.That(lvalue.IsXValue, Is.False);

        var prvalue = body.Children.OfType<IntegerLiteral>().Single();
        Assert.That(prvalue.ValueKind, Is.EqualTo(CX_ExprValueKind.CX_VK_PRValue));
        Assert.That(prvalue.IsPRValue, Is.True);
        Assert.That(prvalue.IsGLValue, Is.False);

        var xvalue = body.Children.OfType<CXXStaticCastExpr>().Single();
        Assert.That(xvalue.ValueKind, Is.EqualTo(CX_ExprValueKind.CX_VK_XValue));
        Assert.That(xvalue.IsXValue, Is.True);
        Assert.That(xvalue.IsGLValue, Is.True);
        Assert.That(xvalue.IsLValue, Is.False);
    }

    [Test]
    public void ObjectKindTest()
    {
        var inputContents = """
struct S
{
    int b : 3;
};

void f(S s)
{
    s.b;
    s;
}
""";

        using var translationUnit = CreateTranslationUnit(inputContents, commandLineArgs: ["-std=c++20"]);

        var func = translationUnit.TranslationUnitDecl.Decls.OfType<FunctionDecl>()
                                  .Single((functionDecl) => functionDecl.Name.Equals("f", StringComparison.Ordinal));
        var body = func.CursorChildren.OfType<CompoundStmt>().Single();

        var bitField = body.Children.OfType<MemberExpr>().Single();
        Assert.That(bitField.ObjectKind, Is.EqualTo(CX_ExprObjectKind.CX_OK_BitField));

        var ordinary = body.Children.OfType<DeclRefExpr>().Single();
        Assert.That(ordinary.ObjectKind, Is.EqualTo(CX_ExprObjectKind.CX_OK_Ordinary));
    }
}
