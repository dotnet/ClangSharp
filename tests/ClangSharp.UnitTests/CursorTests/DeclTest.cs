// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Linq;
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
    public void EnumFieldMethodPredicatesTest()
    {
        var inputContents = """
enum class Scoped : int { A };
enum Plain { B };
struct MyStruct
{
    int : 0;
    int named : 3;
    void lref() &;
    void rref() &&;
    void plain();
};
""";

        using var translationUnit = CreateTranslationUnit(inputContents);

        var scoped = translationUnit.TranslationUnitDecl.Decls.OfType<EnumDecl>().Single((enumDecl) => enumDecl.Name.Equals("Scoped", StringComparison.Ordinal));
        Assert.That(scoped.IsFixed, Is.True);

        var plain = translationUnit.TranslationUnitDecl.Decls.OfType<EnumDecl>().Single((enumDecl) => enumDecl.Name.Equals("Plain", StringComparison.Ordinal));
        Assert.That(plain.IsFixed, Is.False);

        var myStruct = translationUnit.TranslationUnitDecl.Decls.OfType<RecordDecl>().Single((recordDecl) => recordDecl.Name.Equals("MyStruct", StringComparison.Ordinal));

        var fields = myStruct.Decls.OfType<FieldDecl>().ToArray();
        Assert.That(fields[0].IsZeroLengthBitField, Is.True);
        Assert.That(fields[1].IsZeroLengthBitField, Is.False);

        var methods = myStruct.Decls.OfType<CXXMethodDecl>().ToArray();
        Assert.That(methods.Single((method) => method.Name.Equals("lref", StringComparison.Ordinal)).RefQualifier, Is.EqualTo(CXRefQualifierKind.CXRefQualifier_LValue));
        Assert.That(methods.Single((method) => method.Name.Equals("rref", StringComparison.Ordinal)).RefQualifier, Is.EqualTo(CXRefQualifierKind.CXRefQualifier_RValue));
        Assert.That(methods.Single((method) => method.Name.Equals("plain", StringComparison.Ordinal)).RefQualifier, Is.EqualTo(CXRefQualifierKind.CXRefQualifier_None));
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
    public void CXXRecordDeclStructuralPredicatesTest()
    {
        var inputContents = """
struct Pod { };

struct Poly
{
    Poly();
    virtual void f();
    mutable int m;
private:
    int p;
};
""";

        using var translationUnit = CreateTranslationUnit(inputContents);

        var records = translationUnit.TranslationUnitDecl.Decls.OfType<CXXRecordDecl>()
                                     .Where((recordDecl) => recordDecl.HasDefinition)
                                     .ToDictionary((recordDecl) => recordDecl.Name, StringComparer.Ordinal);

        var pod = records["Pod"];
        var poly = records["Poly"];

        Assert.That(pod.IsAggregate, Is.True);
        Assert.That(pod.IsEmpty, Is.True);
        Assert.That(pod.IsPOD, Is.True);
        Assert.That(pod.IsStandardLayout, Is.True);
        Assert.That(pod.IsTrivial, Is.True);
        Assert.That(pod.IsTriviallyCopyable, Is.True);
        Assert.That(pod.IsPolymorphic, Is.False);
        Assert.That(pod.IsDynamicClass, Is.False);
        Assert.That(pod.HasUserDeclaredConstructor, Is.False);

        Assert.That(poly.IsPolymorphic, Is.True);
        Assert.That(poly.IsDynamicClass, Is.True);
        Assert.That(poly.IsAggregate, Is.False);
        Assert.That(poly.IsEmpty, Is.False);
        Assert.That(poly.IsPOD, Is.False);
        Assert.That(poly.IsTrivial, Is.False);
        Assert.That(poly.HasUserDeclaredConstructor, Is.True);
        Assert.That(poly.HasMutableFields, Is.True);
        Assert.That(poly.HasPrivateFields, Is.True);
        Assert.That(poly.HasNonTrivialDestructor, Is.False);
    }

    [Test]
    public void FunctionConstexprKindTest()
    {
        var inputContents = """
constexpr int CxFunc() { return 0; }
consteval int CvFunc() { return 0; }
int PlainFunc() { return 0; }
""";

        string[] commandLineArgs = ["-std=c++20", "-Wno-pragma-once-outside-header"];
        using var translationUnit = CreateTranslationUnit(inputContents, commandLineArgs: commandLineArgs);

        var funcs = translationUnit.TranslationUnitDecl.Decls.OfType<FunctionDecl>()
                                   .ToDictionary((functionDecl) => functionDecl.Name, StringComparer.Ordinal);

        Assert.That(funcs["CxFunc"].ConstexprKind, Is.EqualTo(CX_ConstexprSpecKind.CX_CSK_Constexpr));
        Assert.That(funcs["CxFunc"].IsConstexpr, Is.True);
        Assert.That(funcs["CxFunc"].IsConsteval, Is.False);

        Assert.That(funcs["CvFunc"].ConstexprKind, Is.EqualTo(CX_ConstexprSpecKind.CX_CSK_Consteval));
        Assert.That(funcs["CvFunc"].IsConstexpr, Is.True);
        Assert.That(funcs["CvFunc"].IsConsteval, Is.True);

        Assert.That(funcs["PlainFunc"].ConstexprKind, Is.EqualTo(CX_ConstexprSpecKind.CX_CSK_Unspecified));
        Assert.That(funcs["PlainFunc"].IsConstexpr, Is.False);
        Assert.That(funcs["PlainFunc"].IsConsteval, Is.False);
    }

    [Test]
    public void VarInitStyleTest()
    {
        var inputContents = """
void f()
{
    int cinit = 0;
    int listinit{0};
    int callinit(0);
}
""";

        using var translationUnit = CreateTranslationUnit(inputContents);

        var func = translationUnit.TranslationUnitDecl.Decls.OfType<FunctionDecl>()
                                  .Single((functionDecl) => functionDecl.Name.Equals("f", StringComparison.Ordinal));
        var vars = func.CursorChildren.OfType<CompoundStmt>().Single()
                       .Children.OfType<DeclStmt>()
                       .Select((declStmt) => (VarDecl)declStmt.SingleDecl!)
                       .ToDictionary((varDecl) => varDecl.Name, StringComparer.Ordinal);

        Assert.That(vars["cinit"].InitStyle, Is.EqualTo(CX_InitializationStyle.CX_IS_CInit));
        Assert.That(vars["listinit"].InitStyle, Is.EqualTo(CX_InitializationStyle.CX_IS_ListInit));
        Assert.That(vars["callinit"].InitStyle, Is.EqualTo(CX_InitializationStyle.CX_IS_CallInit));
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

    [Test]
    public void CXXMethodIsPureVirtualTest()
    {
        var inputContents = """
struct S
{
    virtual void PureMethod() = 0;
    virtual void VirtualMethod() {}
    void PlainMethod() {}
};
""";

        using var translationUnit = CreateTranslationUnit(inputContents);

        var recordDecl = translationUnit.TranslationUnitDecl.Decls.OfType<CXXRecordDecl>().Single((decl) => decl.Name.Equals("S", StringComparison.Ordinal));

        CXXMethodDecl Method(string name)
        {
            return recordDecl.Methods.First((method) => method.Name.Equals(name, StringComparison.Ordinal));
        }

        Assert.That(Method("PureMethod").IsPureVirtual, Is.True);
        Assert.That(Method("VirtualMethod").IsPureVirtual, Is.False);
        Assert.That(Method("PlainMethod").IsPureVirtual, Is.False);
    }

    [Test]
    public void FieldOffsetOfFieldTest()
    {
        var inputContents = """
struct S
{
    int first;
    int second;
};
""";

        using var translationUnit = CreateTranslationUnit(inputContents);

        var recordDecl = translationUnit.TranslationUnitDecl.Decls.OfType<CXXRecordDecl>().Single((decl) => decl.Name.Equals("S", StringComparison.Ordinal));

        FieldDecl Field(string name)
        {
            return recordDecl.Fields.First((field) => field.Name.Equals(name, StringComparison.Ordinal));
        }

        Assert.That(Field("first").OffsetOfField, Is.EqualTo(0));
        Assert.That(Field("second").OffsetOfField, Is.EqualTo(32));
    }
}
