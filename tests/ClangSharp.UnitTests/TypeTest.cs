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
}
