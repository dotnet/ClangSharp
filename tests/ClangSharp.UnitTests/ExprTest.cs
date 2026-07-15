// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Linq;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

public sealed class ExprTest : TranslationUnitTest
{
    private static readonly string[] Cpp20CommandLineArgs = ["-std=c++20", "-Wno-pragma-once-outside-header"];

    private static T? FindDescendant<T>(Cursor cursor)
        where T : Cursor
    {
        if (cursor is T match)
        {
            return match;
        }

        foreach (var child in cursor.CursorChildren)
        {
            if (FindDescendant<T>(child) is T found)
            {
                return found;
            }
        }

        return null;
    }

    [Test]
    public void LambdaExprTest()
    {
        var inputContents = """
auto mutableLambda = [](int x) mutable { return x; };
auto constLambda = [](int x) { return x; };
""";

        using var translationUnit = CreateTranslationUnit(inputContents, commandLineArgs: Cpp20CommandLineArgs);

        var vars = translationUnit.TranslationUnitDecl.Decls.OfType<VarDecl>()
                                  .ToDictionary((varDecl) => varDecl.Name, StringComparer.Ordinal);

        var mutableLambda = FindDescendant<LambdaExpr>(vars["mutableLambda"].Init);
        var constLambda = FindDescendant<LambdaExpr>(vars["constLambda"].Init);

        Assert.That(mutableLambda, Is.Not.Null);
        Assert.That(constLambda, Is.Not.Null);

        Assert.That(mutableLambda!.CallOperator, Is.Not.Null);
        Assert.That(mutableLambda.CallOperator.Name, Is.EqualTo("operator()"));

        Assert.That(mutableLambda.IsMutable, Is.True);
        Assert.That(constLambda!.IsMutable, Is.False);
    }

    [Test]
    public void IgnoreExprTest()
    {
        // `int gVar = (gInt);` produces ImplicitCastExpr(LValueToRValue) -> ParenExpr -> DeclRefExpr.
        var inputContents = """
int gInt;
int gVar = (gInt);
""";

        using var translationUnit = CreateTranslationUnit(inputContents);

        var gVar = translationUnit.TranslationUnitDecl.Decls.OfType<VarDecl>().Single((varDecl) => varDecl.Name.Equals("gVar", StringComparison.Ordinal));
        var init = gVar.Init;

        Assert.That(init, Is.InstanceOf<ImplicitCastExpr>(), "init should be an implicit LValueToRValue cast");

        // IgnoreParens leaves the outer implicit cast in place.
        Assert.That(init.IgnoreParens, Is.InstanceOf<ImplicitCastExpr>());

        // IgnoreImpCasts / IgnoreImplicit / IgnoreCasts strip the cast but stop at the ParenExpr.
        Assert.That(init.IgnoreImpCasts, Is.InstanceOf<ParenExpr>());
        Assert.That(init.IgnoreImplicit, Is.InstanceOf<ParenExpr>());
        Assert.That(init.IgnoreCasts, Is.InstanceOf<ParenExpr>());

        // The paren-aware variants strip both the cast and the parens.
        Assert.That(init.IgnoreParenCasts, Is.InstanceOf<DeclRefExpr>());
        Assert.That(init.IgnoreParenImpCasts, Is.InstanceOf<DeclRefExpr>());
        Assert.That(init.IgnoreParenLValueCasts, Is.InstanceOf<DeclRefExpr>());

        // IgnoreParenBaseCasts only strips base casts, so the LValueToRValue cast remains.
        Assert.That(init.IgnoreParenBaseCasts, Is.InstanceOf<ImplicitCastExpr>());

        // IgnoreImplicitAsWritten walks through the implicit cast as written.
        Assert.That(init.IgnoreImplicitAsWritten, Is.Not.InstanceOf<ImplicitCastExpr>());
    }

    [Test]
    public void RewrittenBinaryOperatorIsReversedTest()
    {
        var inputContents = """
struct S { int v; };
bool operator==(const S& lhs, int rhs);

struct T { int v; };
int operator<=>(const T& lhs, const T& rhs);

bool reversedEquals(S s) { return 1 == s; }
bool spaceshipLess(T a, T b) { return a < b; }
""";

        using var translationUnit = CreateTranslationUnit(inputContents, commandLineArgs: Cpp20CommandLineArgs);

        var functions = translationUnit.TranslationUnitDecl.Decls.OfType<FunctionDecl>()
                                       .ToDictionary((functionDecl) => functionDecl.Name, StringComparer.Ordinal);

        var reversed = FindDescendant<CXXRewrittenBinaryOperator>(functions["reversedEquals"]);
        var forward = FindDescendant<CXXRewrittenBinaryOperator>(functions["spaceshipLess"]);

        Assert.That(reversed, Is.Not.Null, "`1 == s` should be a reversed rewritten operator");
        Assert.That(reversed!.IsReversed, Is.True);

        Assert.That(forward, Is.Not.Null, "`a < b` should be a rewritten operator from <=>");
        Assert.That(forward!.IsReversed, Is.False);
    }
}
