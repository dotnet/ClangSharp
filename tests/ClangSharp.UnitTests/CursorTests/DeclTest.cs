// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using ClangSharp.Interop;
using NUnit.Framework;
using static ClangSharp.Interop.CX_CXXAccessSpecifier;
using static ClangSharp.Interop.CX_StmtClass;

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
    public void FunctionTemplateSpecializationArgsTest()
    {
        SkipUntilNativeRebuild();

        var inputContents = $@"template<class T, class U>
void MyFunction(T t, U u);

template<>
void MyFunction<int, float>(int t, float u);
";

        using var translationUnit = CreateTranslationUnit(inputContents);

        var functionDecl = translationUnit.TranslationUnitDecl.Decls.OfType<FunctionDecl>().Single((functionDecl) => functionDecl.TemplateSpecializationArgs.Count != 0);

        Assert.That(functionDecl.TemplateSpecializationArgs.Count, Is.EqualTo(2));
        Assert.That(functionDecl.TemplateSpecializationArgs[0].AsType.AsString, Is.EqualTo("int"));
        Assert.That(functionDecl.TemplateSpecializationArgs[1].AsType.AsString, Is.EqualTo("float"));
    }

    [Test]
    public void VarTemplateSpecializationArgsTest()
    {
        SkipUntilNativeRebuild();

        var inputContents = $@"template<class T, class U>
constexpr int MyVar = 0;

template<>
constexpr int MyVar<int, float> = 1;
";

        using var translationUnit = CreateTranslationUnit(inputContents);

        var varTemplateSpecializationDecl = translationUnit.TranslationUnitDecl.Decls.OfType<VarTemplateSpecializationDecl>().Single();

        Assert.That(varTemplateSpecializationDecl.TemplateArgs.Count, Is.EqualTo(2));
        Assert.That(varTemplateSpecializationDecl.TemplateArgs[0].IsNull, Is.False);
        Assert.That(varTemplateSpecializationDecl.TemplateArgs[1].IsNull, Is.False);
        Assert.That(varTemplateSpecializationDecl.TemplateArgs[0].AsType.AsString, Is.EqualTo("int"));
        Assert.That(varTemplateSpecializationDecl.TemplateArgs[1].AsType.AsString, Is.EqualTo("float"));
    }

    [Test]
    public void UsingEnumDeclTest()
    {
        // `using enum` requires C++20 and previously threw because UsingEnumDecl passed the wrong
        // expectedCursorKind to the base Cursor ctor (libClang surfaces it as CXCursor_EnumDecl).
        var inputContents = """
enum class E { A, B };

struct S {
    using enum E;
};
""";

        string[] commandLineArgs = ["-std=c++20", "-Wno-pragma-once-outside-header"];
        using var translationUnit = CreateTranslationUnit(inputContents, commandLineArgs: commandLineArgs);

        var structDecl = translationUnit.TranslationUnitDecl.Decls.OfType<CXXRecordDecl>().Single((recordDecl) => recordDecl.Name.Equals("S", StringComparison.Ordinal));
        var usingEnumDecl = structDecl.Decls.OfType<UsingEnumDecl>().Single();

        Assert.That(usingEnumDecl.Handle.kind, Is.EqualTo(CXCursorKind.CXCursor_EnumDecl));
        Assert.That(usingEnumDecl.Handle.DeclKind, Is.EqualTo(CX_DeclKind.CX_DeclKind_UsingEnum));
    }

    [Test]
    public void UsingEnumDeclEnumDeclTest()
    {
        // Resolving UsingEnumDecl.EnumDecl relies on the native clangsharp_Cursor_getUsingEnumDeclEnumDecl
        // shim, which the pinned 21.1 prebuilt package predates; skip until the native lib is rebuilt.
        SkipUntilNativeRebuild();

        var inputContents = """
enum class E { A, B };

struct S {
    using enum E;
};
""";

        string[] commandLineArgs = ["-std=c++20", "-Wno-pragma-once-outside-header"];
        using var translationUnit = CreateTranslationUnit(inputContents, commandLineArgs: commandLineArgs);

        var structDecl = translationUnit.TranslationUnitDecl.Decls.OfType<CXXRecordDecl>().Single((recordDecl) => recordDecl.Name.Equals("S", StringComparison.Ordinal));
        var usingEnumDecl = structDecl.Decls.OfType<UsingEnumDecl>().Single();

        var enumDecl = usingEnumDecl.EnumDecl;
        Assert.That(enumDecl, Is.Not.Null);
        Assert.That(enumDecl.Name, Is.EqualTo("E"));
    }

    [Test]
    public void IsPodTest()
    {
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
    public void QualifiedNameTest()
    {
        var inputContents = """
class C {
    void M();
    int F;
};

struct S {
    void M();
    int F;
};
""";

        using var translationUnit = CreateTranslationUnit(inputContents);

        var records = translationUnit.TranslationUnitDecl.Decls.OfType<CXXRecordDecl>().ToArray();
        var classDecl = records.Single(v => v.Name == "C");
        var structDecl = records.Single(v => v.Name == "S");
        var classM = classDecl.Decls.OfType<FunctionDecl>().Single();
        var structM = structDecl.Decls.OfType<FunctionDecl>().Single();

        Assert.That(classDecl.QualifiedName, Is.EqualTo("C"), "Class");
        Assert.That(classM.QualifiedName, Is.EqualTo("C::M"), "Class Method");

        Assert.That(structDecl.QualifiedName, Is.EqualTo("S"), "Struct");
        Assert.That(structM.QualifiedName, Is.EqualTo("S::M"), "Struct Method");
    }

    [Test]
    public void UnsignedValue()
    {
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

            if (initExpr is not IntegerLiteral literalExpr)
            {
                if (initExpr is not { StmtClass: CX_StmtClass_UnaryOperator } subExpr)
                {
                    var castExpr = (ImplicitCastExpr)initExpr!;
                    subExpr = castExpr.SubExpr;
                }

                if (subExpr is UnaryOperator unaryOperator)
                {
                    Assert.That(unaryOperator.Opcode, Is.EqualTo(CXUnaryOperatorKind.CXUnaryOperator_Minus), $"enum E::{fieldName} InitExpr is not a minus UnaryOperator");
                    subExpr = unaryOperator.SubExpr;
                    isNegativeExpression = true;
                }

                literalExpr = (IntegerLiteral)subExpr;
            }
            Assert.That(literalExpr, Is.Not.Null, $"enum E::{fieldName} InitExpr is not IntegerLiteral {initExpr?.GetType().Name}");
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

    // Some tests depend on native libClangSharp shims that the pinned 21.1 prebuilt package predates
    // (e.g. clangsharp_Cursor_getNumTemplateArguments / getTemplateArgument and
    // clangsharp_Cursor_getUsingEnumDeclEnumDecl). Skip those until the native lib is rebuilt for a
    // newer libClang. Rebuilding off 21.1 auto-unskips them.
    private static void SkipUntilNativeRebuild()
    {
        using var versionString = clang.getClangVersion();
        var match = Regex.Match(versionString.ToString(), @"version (\d+)\.(\d+)");

        if (match.Success
            && (int.Parse(match.Groups[1].ValueSpan, CultureInfo.InvariantCulture) == 21)
            && (int.Parse(match.Groups[2].ValueSpan, CultureInfo.InvariantCulture) == 1))
        {
            Assert.Ignore("Requires a native libClangSharp rebuild that includes the template-argument accessor fix; the pinned 21.1 prebuilt package predates it. Remove this guard once libClang moves off 21.1 and the native lib is rebuilt.");
        }
    }
}
