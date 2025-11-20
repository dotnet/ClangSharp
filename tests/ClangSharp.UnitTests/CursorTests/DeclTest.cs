// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Linq;
using ClangSharp.Interop;
using NUnit.Framework;
using static ClangSharp.Interop.CX_CXXAccessSpecifier;

namespace ClangSharp.UnitTests;

public sealed class DeclTest : TranslationUnitTest
{
    [TestCase("private", CX_CXXPrivate)]
    [TestCase("protected", CX_CXXProtected)]
    [TestCase("public", CX_CXXPublic)]
    public void AccessSpecDeclTest(string accessSpecifier, CX_CXXAccessSpecifier expectedAccessSpecifier)
    {
        var inputContents = $@"struct MyStruct
{{
{accessSpecifier}:
}};
";

        using var translationUnit = CreateTranslationUnit(inputContents);

        var recordDecl = translationUnit.TranslationUnitDecl.Decls.OfType<RecordDecl>().Where((recordDecl) => recordDecl.Name.Equals("MyStruct", StringComparison.Ordinal)).Single();
        var accessSpecDecl = recordDecl.Decls.OfType<AccessSpecDecl>().Single();

        Assert.That(accessSpecDecl.Access, Is.EqualTo(expectedAccessSpecifier));
    }

    [Test]
    public void ClassTemplateDeclTest()
    {
        var inputContents = $@"template<class T>
class MyClass
{{
    T value;
}};
";

        using var translationUnit = CreateTranslationUnit(inputContents);

        var classTemplateDecl = translationUnit.TranslationUnitDecl.Decls.OfType<ClassTemplateDecl>().Single();
        Assert.That(classTemplateDecl.Name, Is.EqualTo("MyClass"));

        var templateParameter = classTemplateDecl.TemplateParameters.Single();
        Assert.That(templateParameter.Name, Is.EqualTo("T"));
    }

    [Test]
    public void ClassTemplatePartialSpecializationDeclTest()
    {
        var inputContents = $@"template<class T, class U>
class MyClass
{{
    T value1;
    U value2;
}};

template<class U>
class MyClass<int, U>
{{
    int value1;
    U value2;
}};
";

        using var translationUnit = CreateTranslationUnit(inputContents);

        var classTemplatePartialSpecializationDecl = translationUnit.TranslationUnitDecl.Decls.OfType<ClassTemplatePartialSpecializationDecl>().Single();
        Assert.That(classTemplatePartialSpecializationDecl.Name, Is.EqualTo("MyClass"));

        var templateParameter = classTemplatePartialSpecializationDecl.TemplateParameters.Single();
        Assert.That(templateParameter.Name, Is.EqualTo("U"));
    }

    [Test]
    public void TemplateParameterPackTest()
    {
        var inputContents = $@"template<class... Types>
class tuple;

tuple<int, long> SomeFunction();
";

        using var translationUnit = CreateTranslationUnit(inputContents);

        var functionDecl = translationUnit.TranslationUnitDecl.Decls.OfType<FunctionDecl>().Single();
        var tupleDecl = functionDecl.ReturnType.AsCXXRecordDecl as ClassTemplateSpecializationDecl;
        Assert.That(tupleDecl, Is.Not.Null);
        Assert.That(tupleDecl!.TemplateArgs.Count, Is.EqualTo(1));

        var packElements = tupleDecl.TemplateArgs[0].PackElements;
        Assert.That(packElements.Count, Is.EqualTo(2));
        Assert.That(packElements[0].AsType.AsString, Is.EqualTo("int"));
        Assert.That(packElements[1].AsType.AsString, Is.EqualTo("long"));
    }

    [Test]
    public void FloatingLiteral()
    {
        ObjectiveCTest.AssertNeedNewClangSharp();

        var inputContents = $$"""
const float F1 = 3.14f;
const double D1 = 2.718281828;
const long double Q1 = 1.61803398874989484820458683436563811772030917980576286213544862270526046281890244L;
""";

        using var translationUnit = CreateTranslationUnit(inputContents);

        var decls = translationUnit.TranslationUnitDecl.Decls.OfType<VarDecl>().ToList();

        var checkConstant = (string constantName, float? expectedFloat, double? expectedDouble, double expectedApproximateDouble) => {
            var constant = decls.SingleOrDefault(e => e.Name == constantName)!;
            var expr = constant.Init!;
            if (expr is ImplicitCastExpr castExpr)
            {
                expr = castExpr.SubExpr;
            }
            if (expr is FloatingLiteral floatingLiteral)
            {
                Assert.That(floatingLiteral.ValueAsFloat, Is.EqualTo(expectedFloat).Within(0.0001), $"Constant {constantName} float value mismatch");
                Assert.That(floatingLiteral.ValueAsDouble, Is.EqualTo(expectedDouble).Within(0.0001), $"Constant {constantName} double value mismatch");
                Assert.That(floatingLiteral.ValueAsApproximateDouble, Is.EqualTo(expectedApproximateDouble).Within(0.0001), $"Constant {constantName} double value mismatch");
            }
            else
            {
                Assert.Fail($"Constant {constantName} InitExpr is not FloatingLiteral, but {expr.GetType().Name}");
            }
        };

        Assert.Multiple(() => {
            checkConstant("F1", 3.14f, 3.14, 3.14);
            checkConstant("D1", null, 2.718281828, 2.718281828);
            checkConstant("Q1", null, 1.61803398874989484820458683436563811772030917980576, 1.61803398874989484820458683436563811772030917980576);
        });
    }

    [Test]
    public void IsPodTest()
    {
        AssertNeedNewClangSharp();

        var inputContents = $$"""
struct A {
    int a;
};
struct B {
    int b;
private:
    int p;
};
""";

        using var translationUnit = CreateTranslationUnit(inputContents);

        var decls = translationUnit.TranslationUnitDecl.Decls.OfType<CXXRecordDecl>().ToList();

        var structA = decls.SingleOrDefault(d => d.Name == "A")!;
        Assert.That(structA, Is.Not.Null, "struct A not found");
        Assert.That(structA.IsPOD, Is.True, "struct A should be POD");

        var structB = decls.SingleOrDefault(d => d.Name == "B")!;
        Assert.That(structB, Is.Not.Null, "struct B not found");
        Assert.That(structB.IsPOD, Is.False, "struct B should be not POD");
    }
}
