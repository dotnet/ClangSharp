// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Linq;
using ClangSharp.Interop;
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

    private static void CollectDescendants<T>(Cursor cursor, System.Collections.Generic.List<T> results)
        where T : Cursor
    {
        if (cursor is T match)
        {
            results.Add(match);
        }

        foreach (var child in cursor.CursorChildren)
        {
            CollectDescendants(child, results);
        }
    }

    [Test]
    public void LambdaExprTest()
    {
        var inputContents = """
void f()
{
    int gInt = 0;
    auto mutableLambda = [](int x) mutable { return x; };
    auto constLambda = [](int x) { return x; };
    auto genericLambda = [](auto x) { return x; };
    auto byCopyLambda = [=]() { return gInt; };
    auto byRefLambda = [&]() { return gInt; };
}
""";

        using var translationUnit = CreateTranslationUnit(inputContents, commandLineArgs: Cpp20CommandLineArgs);

        var func = translationUnit.TranslationUnitDecl.Decls.OfType<FunctionDecl>()
                                   .Single((functionDecl) => functionDecl.Name.Equals("f", StringComparison.Ordinal));

        var varDecls = new System.Collections.Generic.List<VarDecl>();
        CollectDescendants(func, varDecls);
        var vars = varDecls.Where((varDecl) => varDecl is not ParmVarDecl)
                           .ToDictionary((varDecl) => varDecl.Name, StringComparer.Ordinal);

        var mutableLambda = FindDescendant<LambdaExpr>(vars["mutableLambda"].Init);
        var constLambda = FindDescendant<LambdaExpr>(vars["constLambda"].Init);
        var genericLambda = FindDescendant<LambdaExpr>(vars["genericLambda"].Init);
        var byCopyLambda = FindDescendant<LambdaExpr>(vars["byCopyLambda"].Init);
        var byRefLambda = FindDescendant<LambdaExpr>(vars["byRefLambda"].Init);

        Assert.That(mutableLambda, Is.Not.Null);
        Assert.That(constLambda, Is.Not.Null);
        Assert.That(genericLambda, Is.Not.Null);
        Assert.That(byCopyLambda, Is.Not.Null);
        Assert.That(byRefLambda, Is.Not.Null);

        Assert.That(mutableLambda!.CallOperator, Is.Not.Null);
        Assert.That(mutableLambda.CallOperator.Name, Is.EqualTo("operator()"));
        Assert.That(mutableLambda.LambdaClass, Is.EqualTo(mutableLambda.Type.AsCXXRecordDecl));

        Assert.That(mutableLambda.IsMutable, Is.True);
        Assert.That(constLambda!.IsMutable, Is.False);

        Assert.That(mutableLambda.IsGenericLambda, Is.False);
        Assert.That(genericLambda!.IsGenericLambda, Is.True);

        Assert.That(mutableLambda.IsCapturelessLambda, Is.True);
        Assert.That(byCopyLambda!.IsCapturelessLambda, Is.False);

        Assert.That(mutableLambda.CaptureDefault, Is.EqualTo(CX_LambdaCaptureDefault.CX_LCD_None));
        Assert.That(byCopyLambda.CaptureDefault, Is.EqualTo(CX_LambdaCaptureDefault.CX_LCD_ByCopy));
        Assert.That(byRefLambda!.CaptureDefault, Is.EqualTo(CX_LambdaCaptureDefault.CX_LCD_ByRef));
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
