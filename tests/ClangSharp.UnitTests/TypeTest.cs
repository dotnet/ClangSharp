// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Linq;
using ClangSharp.Interop;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

public sealed class TypeTest : TranslationUnitTest
{
    [Test]
    public void UnqualifiedTypeTest()
    {
        // Regression test for dotnet/ClangSharp#562. A `Type` mirrors a Clang `QualType`, so the
        // local `const` cannot be stripped via sugar removal (`UnqualifiedDesugaredType` keeps it).
        // `UnqualifiedType` exposes `clang_getUnqualifiedType` to drop the local qualifiers.
        var inputContents = """
typedef int const* pcint;
pcint g();
""";

        using var translationUnit = CreateTranslationUnit(inputContents);

        var functionDecl = translationUnit.TranslationUnitDecl.Decls.OfType<FunctionDecl>().Single((functionDecl) => functionDecl.Name.Equals("g", StringComparison.Ordinal));
        var pointeeType = functionDecl.ReturnType.CanonicalType.PointeeType;

        Assert.That(pointeeType.IsLocalConstQualified, Is.True, "pointee should be const-qualified");
        Assert.That(pointeeType.UnqualifiedDesugaredType.IsLocalConstQualified, Is.True, "UnqualifiedDesugaredType keeps the local const");

        var unqualifiedType = pointeeType.UnqualifiedType;

        Assert.That(unqualifiedType.IsLocalConstQualified, Is.False, "UnqualifiedType should drop the local const");
        Assert.That(unqualifiedType.AsString, Is.EqualTo("int"));
    }

    [Test]
    public void PredicatesTest()
    {
        var inputContents = """
struct S { int field; };
enum E { AValue };

using VoidT = void;
using BoolT = bool;
using NullPtrT = decltype(nullptr);
using LRefT = int&;
using RRefT = int&&;
using ArrT = int[4];
using PtrT = int*;
using RecordT = S;
using EnumT = E;
using FuncT = void();
using MemPtrT = int S::*;
using VecT = int __attribute__((vector_size(16)));
""";

        using var translationUnit = CreateTranslationUnit(inputContents);

        var typedefs = translationUnit.TranslationUnitDecl.Decls.OfType<TypedefNameDecl>()
                                      .ToDictionary((typedef) => typedef.Name, (typedef) => typedef.UnderlyingType, StringComparer.Ordinal);

        Assert.That(typedefs["VoidT"].IsVoidType, Is.True);
        Assert.That(typedefs["VoidT"].IsIntegerType, Is.False);

        Assert.That(typedefs["BoolT"].IsBooleanType, Is.True);
        Assert.That(typedefs["NullPtrT"].IsNullPtrType, Is.True);

        Assert.That(typedefs["LRefT"].IsReferenceType, Is.True);
        Assert.That(typedefs["LRefT"].IsLValueReferenceType, Is.True);
        Assert.That(typedefs["LRefT"].IsRValueReferenceType, Is.False);

        Assert.That(typedefs["RRefT"].IsReferenceType, Is.True);
        Assert.That(typedefs["RRefT"].IsRValueReferenceType, Is.True);
        Assert.That(typedefs["RRefT"].IsLValueReferenceType, Is.False);

        Assert.That(typedefs["ArrT"].IsArrayType, Is.True);
        Assert.That(typedefs["PtrT"].IsPointerType, Is.True);
        Assert.That(typedefs["PtrT"].IsAnyPointerType, Is.True);
        Assert.That(typedefs["RecordT"].IsRecordType, Is.True);
        Assert.That(typedefs["EnumT"].IsEnumeralType, Is.True);
        Assert.That(typedefs["FuncT"].IsFunctionType, Is.True);
        Assert.That(typedefs["MemPtrT"].IsMemberPointerType, Is.True);
        Assert.That(typedefs["VecT"].IsVectorType, Is.True);
    }

    [Test]
    public void MemberPointerClassTypeTest()
    {
        var inputContents = """
struct S { int field; };
using MemPtrT = int S::*;
""";

        using var translationUnit = CreateTranslationUnit(inputContents);

        var memPtr = translationUnit.TranslationUnitDecl.Decls.OfType<TypedefNameDecl>().Single((typedef) => typedef.Name.Equals("MemPtrT", StringComparison.Ordinal));
        var memberPointerType = (MemberPointerType)memPtr.UnderlyingType.CanonicalType;

        Assert.That(memberPointerType.ClassType.AsCXXRecordDecl, Is.Not.Null);
        Assert.That(memberPointerType.ClassType.AsCXXRecordDecl!.Name, Is.EqualTo("S"));
    }

    [Test]
    public void CharTypeTest()
    {
        var inputContents = """
using CharT = char;
using UCharT = unsigned char;
using SCharT = signed char;
using WCharT = wchar_t;
using IntT = int;
""";

        using var translationUnit = CreateTranslationUnit(inputContents);

        var typedefs = translationUnit.TranslationUnitDecl.Decls.OfType<TypedefNameDecl>()
                                      .ToDictionary((typedef) => typedef.Name, (typedef) => typedef.UnderlyingType, StringComparer.Ordinal);

        Assert.That(typedefs["CharT"].IsCharType, Is.True);
        Assert.That(typedefs["UCharT"].IsCharType, Is.True);
        Assert.That(typedefs["SCharT"].IsCharType, Is.True);
        Assert.That(typedefs["WCharT"].IsCharType, Is.False);
        Assert.That(typedefs["IntT"].IsCharType, Is.False);
    }

    [Test]
    public void MatrixTypeTest()
    {
        var inputContents = """
typedef float Matrix4x4 __attribute__((matrix_type(4, 4)));
using IntT = int;
""";

        string[] commandLineArgs = ["-std=c++17", "-Wno-pragma-once-outside-header", "-fenable-matrix"];

        using var translationUnit = CreateTranslationUnit(inputContents, commandLineArgs: commandLineArgs);

        var typedefs = translationUnit.TranslationUnitDecl.Decls.OfType<TypedefNameDecl>()
                                      .ToDictionary((typedef) => typedef.Name, (typedef) => typedef.UnderlyingType, StringComparer.Ordinal);

        Assert.That(typedefs["Matrix4x4"].IsMatrixType, Is.True);
        Assert.That(typedefs["IntT"].IsMatrixType, Is.False);
    }

    [Test]
    public void MemberPointerKindTest()
    {
        var inputContents = """
struct S { int field; void method(); };
using MemDataT = int S::*;
using MemFuncT = void (S::*)();
""";

        using var translationUnit = CreateTranslationUnit(inputContents);

        var memDataT = translationUnit.TranslationUnitDecl.Decls.OfType<TypedefNameDecl>().Single((typedef) => typedef.Name.Equals("MemDataT", StringComparison.Ordinal));
        var memFuncT = translationUnit.TranslationUnitDecl.Decls.OfType<TypedefNameDecl>().Single((typedef) => typedef.Name.Equals("MemFuncT", StringComparison.Ordinal));

        var memberDataPointer = (MemberPointerType)memDataT.UnderlyingType.CanonicalType;
        var memberFunctionPointer = (MemberPointerType)memFuncT.UnderlyingType.CanonicalType;

        Assert.That(memberDataPointer.IsMemberDataPointer, Is.True);
        Assert.That(memberDataPointer.IsMemberFunctionPointer, Is.False);

        Assert.That(memberFunctionPointer.IsMemberFunctionPointer, Is.True);
        Assert.That(memberFunctionPointer.IsMemberDataPointer, Is.False);
    }

    [Test]
    public void TemplateNameTest()
    {
        // Resolving TemplateSpecializationType.TemplateName relies on the native clangsharp shim, which
        // the win-arm64 prebuilt returns invalid; skip until the native lib is rebuilt.
        SkipUntilNativeRebuild();

        var inputContents = """
template <typename T> struct Tmpl { T value; };
using SpecT = Tmpl<int>;
""";

        using var translationUnit = CreateTranslationUnit(inputContents);

        var spec = translationUnit.TranslationUnitDecl.Decls.OfType<TypedefNameDecl>().Single((typedef) => typedef.Name.Equals("SpecT", StringComparison.Ordinal));
        var templateSpecializationType = (TemplateSpecializationType)spec.UnderlyingType;
        var templateName = templateSpecializationType.TemplateName;

        Assert.That(templateName.IsNull, Is.False);
        Assert.That(templateName.Kind, Is.EqualTo(CX_TemplateNameKind.CX_TNK_QualifiedTemplate));
        Assert.That(templateName.IsDependent, Is.False);
        Assert.That(templateName.IsInstantiationDependent, Is.False);
        Assert.That(templateName.ContainsUnexpandedParameterPack, Is.False);

        var underlying = templateName.Underlying;
        Assert.That(underlying.IsNull, Is.False);
        Assert.That(underlying.AsTemplateDecl.Name, Is.EqualTo("Tmpl"));
    }

    [Test]
    public void NonReferenceTypeTest()
    {
        var inputContents = """
using LRefT = int&;
using RRefT = int&&;
using ValT = int;
""";

        using var translationUnit = CreateTranslationUnit(inputContents);

        var typedefs = translationUnit.TranslationUnitDecl.Decls.OfType<TypedefNameDecl>().ToDictionary((typedef) => typedef.Name, StringComparer.Ordinal);

        Assert.That(typedefs["LRefT"].UnderlyingType.NonReferenceType.AsString, Is.EqualTo("int"));
        Assert.That(typedefs["RRefT"].UnderlyingType.NonReferenceType.AsString, Is.EqualTo("int"));
        Assert.That(typedefs["ValT"].UnderlyingType.NonReferenceType.AsString, Is.EqualTo("int"));

        Assert.That(typedefs["ValT"].UnderlyingType.AddressSpace, Is.EqualTo(0u));
    }

    [Test]
    public void ElaboratedKeywordTest()
    {
        // clang 22 removed ElaboratedType; the elaboration keyword now lives on each keyword-bearing
        // type (TagType/TypedefType/UsingType/...) and is surfaced via TypeWithKeyword.Keyword.
        var inputContents = """
struct S {};
enum E { AValue };

struct S sVar;
enum E eVar;
S plainVar;
""";

        using var translationUnit = CreateTranslationUnit(inputContents);

        var vars = translationUnit.TranslationUnitDecl.Decls.OfType<VarDecl>()
                                   .ToDictionary((varDecl) => varDecl.Name, (varDecl) => (TypeWithKeyword)varDecl.Type, StringComparer.Ordinal);

        Assert.That(vars["sVar"].Keyword, Is.EqualTo(CX_ElaboratedTypeKeyword.CX_ETK_Struct));
        Assert.That(vars["eVar"].Keyword, Is.EqualTo(CX_ElaboratedTypeKeyword.CX_ETK_Enum));
        Assert.That(vars["plainVar"].Keyword, Is.EqualTo(CX_ElaboratedTypeKeyword.CX_ETK_None));
    }

    [Test]
    public void DependenceTest()
    {
        var inputContents = """
template <typename T> T Identity(T value);
int Plain;
""";

        using var translationUnit = CreateTranslationUnit(inputContents);

        var functionTemplate = translationUnit.TranslationUnitDecl.Decls.OfType<FunctionTemplateDecl>().Single();
        var dependentType = functionTemplate.TemplatedDecl.ReturnType;

        Assert.That(dependentType.IsDependentType, Is.True);
        Assert.That(dependentType.IsInstantiationDependentType, Is.True);
        Assert.That(dependentType.IsVariablyModifiedType, Is.False);
        Assert.That(dependentType.ContainsUnexpandedParameterPack, Is.False);
        Assert.That(dependentType.ContainsErrors, Is.False);

        var plainType = translationUnit.TranslationUnitDecl.Decls.OfType<VarDecl>().Single((varDecl) => varDecl.Name.Equals("Plain", StringComparison.Ordinal)).Type;

        Assert.That(plainType.Dependence, Is.EqualTo(CX_TypeDependence.CX_TD_None));
        Assert.That(plainType.IsDependentType, Is.False);
    }

    [Test]
    public void AutoTypeKeywordTest()
    {
        var inputContents = """
template <typename T> auto Deduced(T value);
""";

        using var translationUnit = CreateTranslationUnit(inputContents);

        var functionTemplate = translationUnit.TranslationUnitDecl.Decls.OfType<FunctionTemplateDecl>().Single();
        var autoType = (AutoType)functionTemplate.TemplatedDecl.ReturnType;

        Assert.That(autoType.Keyword, Is.EqualTo(CX_AutoTypeKeyword.CX_ATK_Auto));
    }

    [Test]
    public void ClassificationPredicatesTest()
    {
        var inputContents = """
struct S { int field; };

using IntT = int;
using FloatT = float;
using PtrT = int*;
using StructT = S;
using VoidT = void;
""";

        using var translationUnit = CreateTranslationUnit(inputContents);

        var typedefs = translationUnit.TranslationUnitDecl.Decls.OfType<TypedefNameDecl>()
                                      .ToDictionary((typedef) => typedef.Name, (typedef) => typedef.UnderlyingType, StringComparer.Ordinal);

        var intT = typedefs["IntT"];
        Assert.That(intT.IsScalarType, Is.True);
        Assert.That(intT.IsArithmeticType, Is.True);
        Assert.That(intT.IsObjectType, Is.True);
        Assert.That(intT.IsAggregateType, Is.False);
        Assert.That(intT.IsFloatingType, Is.False);
        Assert.That(intT.HasIntegerRepresentation, Is.True);
        Assert.That(intT.HasPointerRepresentation, Is.False);

        var floatT = typedefs["FloatT"];
        Assert.That(floatT.IsFloatingType, Is.True);
        Assert.That(floatT.IsRealFloatingType, Is.True);
        Assert.That(floatT.IsArithmeticType, Is.True);
        Assert.That(floatT.HasFloatingRepresentation, Is.True);
        Assert.That(floatT.HasIntegerRepresentation, Is.False);

        var ptrT = typedefs["PtrT"];
        Assert.That(ptrT.IsScalarType, Is.True);
        Assert.That(ptrT.IsArithmeticType, Is.False);
        Assert.That(ptrT.IsObjectType, Is.True);
        Assert.That(ptrT.HasPointerRepresentation, Is.True);

        var structT = typedefs["StructT"];
        Assert.That(structT.IsAggregateType, Is.True);
        Assert.That(structT.IsScalarType, Is.False);
        Assert.That(structT.IsObjectType, Is.True);

        var voidT = typedefs["VoidT"];
        Assert.That(voidT.IsObjectType, Is.False);
        Assert.That(voidT.IsScalarType, Is.False);
    }

    [Test]
    public void VectorKindTest()
    {
        var inputContents = """
using VecT = int __attribute__((vector_size(16)));
""";

        using var translationUnit = CreateTranslationUnit(inputContents);

        var vecT = translationUnit.TranslationUnitDecl.Decls.OfType<TypedefNameDecl>().Single((typedef) => typedef.Name.Equals("VecT", StringComparison.Ordinal)).UnderlyingType;
        var vectorType = (VectorType)vecT.CanonicalType;

        Assert.That(vectorType.VectorKind, Is.EqualTo(CX_VectorKind.CX_VECK_Generic));
    }

    [Test]
    public void FunctionProtoTypeTest()
    {
        var inputContents = """
int simple();
auto trailing() -> int;
void noThrow() noexcept;
template <typename T> void mayThrow() noexcept(sizeof(T) > 1);
""";

        using var translationUnit = CreateTranslationUnit(inputContents);

        var functions = translationUnit.TranslationUnitDecl.Decls.OfType<FunctionDecl>()
                                       .ToDictionary((functionDecl) => functionDecl.Name, (functionDecl) => (FunctionProtoType)functionDecl.Type, StringComparer.Ordinal);

        var simple = functions["simple"];
        Assert.That(simple.HasTrailingReturn, Is.False);
        Assert.That(simple.ExceptionSpecType, Is.EqualTo(CXCursor_ExceptionSpecificationKind.CXCursor_ExceptionSpecificationKind_None));
        Assert.That(simple.NoexceptExpr, Is.Null);

        Assert.That(functions["trailing"].HasTrailingReturn, Is.True);

        var noThrow = functions["noThrow"];
        Assert.That(noThrow.ExceptionSpecType, Is.EqualTo(CXCursor_ExceptionSpecificationKind.CXCursor_ExceptionSpecificationKind_BasicNoexcept));
        Assert.That(noThrow.NoexceptExpr, Is.Null);

        var mayThrow = (FunctionProtoType)translationUnit.TranslationUnitDecl.Decls.OfType<FunctionTemplateDecl>().Single().TemplatedDecl.Type;
        Assert.That(mayThrow.NoexceptExpr, Is.Not.Null);
    }

    [Test]
    public void AutoTypePredicatesTest()
    {
        var inputContents = """
template <typename T> auto Plain(T value);
template <typename T> decltype(auto) Decltype(T value);
template <typename T> concept Small = sizeof(T) > 0;
template <typename T> Small auto Constrained(T value);
""";

        using var translationUnit = CreateTranslationUnit(inputContents, commandLineArgs: Cpp20CommandLineArgs);

        var autoTypes = translationUnit.TranslationUnitDecl.Decls.OfType<FunctionTemplateDecl>()
                                       .ToDictionary((template) => template.Name, (template) => (AutoType)template.TemplatedDecl.ReturnType, StringComparer.Ordinal);

        var plain = autoTypes["Plain"];
        Assert.That(plain.IsConstrained, Is.False);
        Assert.That(plain.IsDecltypeAuto, Is.False);
        Assert.That(plain.IsGNUAutoType, Is.False);

        Assert.That(autoTypes["Decltype"].IsDecltypeAuto, Is.True);
        Assert.That(autoTypes["Constrained"].IsConstrained, Is.True);
    }

    private static readonly string[] Cpp20CommandLineArgs = ["-std=c++20", "-Wno-pragma-once-outside-header"];
}
