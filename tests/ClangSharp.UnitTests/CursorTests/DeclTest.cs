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

    [Test]
    public void UnsignedValue()
    {
        ObjectiveCTest.AssertNeedNewClangSharp();

        var inputContents = $$"""
enum E {
    A = 1,
    B = 4294967295U,
    C = 4294967296U,
    D = 18446744073709551615ULL,
    E = -1,
    F = -4294967295,
    G = -4294967296,
    H = -18446744073709551615LL,
};
""";

        using var translationUnit = CreateTranslationUnit(inputContents);

        var decls = translationUnit.TranslationUnitDecl.Decls.OfType<EnumDecl>().ToList();

        var enumE = decls.SingleOrDefault(d => d.Name == "E")!;
        Assert.That(enumE, Is.Not.Null, "enum E not found");

        var checkField = (string fieldName, long expectedValue, ulong expectedUnsignedValue, bool negative) => {
            var field = enumE.Enumerators.SingleOrDefault(e => e.Name == fieldName)!;
            Assert.That(field, Is.Not.Null, $"enum E::{fieldName} not found");
            var initExpr = field.InitExpr;
            Assert.That(initExpr, Is.Not.Null, $"enum E::{fieldName} InitExpr is null");

            var isNegativeExpression = false;
            var castExpr = (ImplicitCastExpr)initExpr!;
            var subExpr = castExpr.SubExpr;
            if (subExpr is UnaryOperator unaryOperator)
            {
                Assert.That(unaryOperator.Opcode, Is.EqualTo(CXUnaryOperatorKind.CXUnaryOperator_Minus), $"enum E::{fieldName} InitExpr is not a minus UnaryOperator");
                subExpr = unaryOperator.SubExpr;
                isNegativeExpression = true;
            }
            var literalExpr = subExpr as IntegerLiteral;
            Assert.That(literalExpr, Is.Not.Null, $"enum E::{fieldName} InitExpr is not IntegerLiteral {castExpr.SubExpr!.GetType().Name}");
            Assert.That(literalExpr!.Value, Is.EqualTo(expectedValue), $"enum E::{fieldName} value mismatch");
            Assert.That(literalExpr!.UnsignedValue, Is.EqualTo(expectedUnsignedValue), $"enum E::{fieldName} unsigned value mismatch");
            Assert.That(negative, Is.EqualTo(isNegativeExpression), $"enum E::{fieldName} negative mismatch");
        };

        Assert.Multiple(() => {
            checkField("A", 1, 1, false);
            checkField("B", -1, 4294967295UL, false);
            checkField("C", 4294967296, 4294967296UL, false);
            checkField("D", -1, 18446744073709551615UL, false);
            checkField("E", 1, 1, true);
            checkField("F", 4294967295, 4294967295, true);
            checkField("G", 4294967296, 4294967296, true);
            checkField("H", -1, 18446744073709551615UL, true);
        });
    }
}
