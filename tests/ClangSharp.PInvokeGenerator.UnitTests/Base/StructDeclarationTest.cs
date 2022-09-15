// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

public abstract class StructDeclarationTest : PInvokeGeneratorTest
{
    [TestCase("double", "double")]
    [TestCase("short", "short")]
    [TestCase("int", "int")]
    [TestCase("float", "float")]
    public Task IncompleteArraySizeTest(string nativeType, string expectedManagedType) => IncompleteArraySizeTestImpl(nativeType, expectedManagedType);

    [TestCase("double", "double")]
    [TestCase("short", "short")]
    [TestCase("int", "int")]
    [TestCase("float", "float")]
    public Task BasicTest(string nativeType, string expectedManagedType) => BasicTestImpl(nativeType, expectedManagedType);

    [TestCase("double", "double")]
    [TestCase("short", "short")]
    [TestCase("int", "int")]
    [TestCase("float", "float")]
    public Task BasicTestInCMode(string nativeType, string expectedManagedType) => BasicTestInCModeImpl(nativeType, expectedManagedType);

    [TestCase("unsigned char", "byte")]
    [TestCase("long long", "long")]
    [TestCase("signed char", "sbyte")]
    [TestCase("unsigned short", "ushort")]
    [TestCase("unsigned int", "uint")]
    [TestCase("unsigned long long", "ulong")]
    [TestCase("bool", "byte")]
    public Task BasicWithNativeTypeNameTest(string nativeType, string expectedManagedType) => BasicWithNativeTypeNameTestImpl(nativeType, expectedManagedType);

    [Test]
    public Task BitfieldTest() => BitfieldTestImpl();

    [Test]
    public Task ExcludeTest() => ExcludeTestImpl();

    [TestCase("double", "double")]
    [TestCase("short", "short")]
    [TestCase("int", "int")]
    [TestCase("float", "float")]
    public Task FixedSizedBufferNonPrimitiveTest(string nativeType, string expectedManagedType) => FixedSizedBufferNonPrimitiveTestImpl(nativeType, expectedManagedType);

    [TestCase("double", "double")]
    [TestCase("short", "short")]
    [TestCase("int", "int")]
    [TestCase("float", "float")]
    public Task FixedSizedBufferNonPrimitiveMultidimensionalTest(string nativeType, string expectedManagedType) => FixedSizedBufferNonPrimitiveMultidimensionalTestImpl(nativeType, expectedManagedType);

    [TestCase("double", "double")]
    [TestCase("short", "short")]
    [TestCase("int", "int")]
    [TestCase("float", "float")]
    public Task FixedSizedBufferNonPrimitiveTypedefTest(string nativeType, string expectedManagedType) => FixedSizedBufferNonPrimitiveTypedefTestImpl(nativeType, expectedManagedType);

    [TestCase("unsigned char", "byte")]
    [TestCase("long long", "long")]
    [TestCase("signed char", "sbyte")]
    [TestCase("unsigned short", "ushort")]
    [TestCase("unsigned int", "uint")]
    [TestCase("unsigned long long", "ulong")]
    [TestCase("bool", "byte")]
    public Task FixedSizedBufferNonPrimitiveWithNativeTypeNameTest(string nativeType, string expectedManagedType) => FixedSizedBufferNonPrimitiveWithNativeTypeNameTestImpl(nativeType, expectedManagedType);

    [TestCase("double *", "double*")]
    [TestCase("short *", "short*")]
    [TestCase("int *", "int*")]
    [TestCase("float *", "float*")]
    public Task FixedSizedBufferPointerTest(string nativeType, string expectedManagedType) => FixedSizedBufferPointerTestImpl(nativeType, expectedManagedType);

    [TestCase("unsigned char", "byte")]
    [TestCase("double", "double")]
    [TestCase("short", "short")]
    [TestCase("int", "int")]
    [TestCase("long long", "long")]
    [TestCase("signed char", "sbyte")]
    [TestCase("float", "float")]
    [TestCase("unsigned short", "ushort")]
    [TestCase("unsigned int", "uint")]
    [TestCase("unsigned long long", "ulong")]
    [TestCase("bool", "byte")]
    public Task FixedSizedBufferPrimitiveTest(string nativeType, string expectedManagedType) => FixedSizedBufferPrimitiveTestImpl(nativeType, expectedManagedType);

    [TestCase("unsigned char", "byte")]
    [TestCase("double", "double")]
    [TestCase("short", "short")]
    [TestCase("int", "int")]
    [TestCase("long long", "long")]
    [TestCase("signed char", "sbyte")]
    [TestCase("float", "float")]
    [TestCase("unsigned short", "ushort")]
    [TestCase("unsigned int", "uint")]
    [TestCase("unsigned long long", "ulong")]
    [TestCase("bool", "byte")]
    public Task FixedSizedBufferPrimitiveMultidimensionalTest(string nativeType, string expectedManagedType) => FixedSizedBufferPrimitiveMultidimensionalTestImpl(nativeType, expectedManagedType);

    [TestCase("unsigned char", "byte")]
    [TestCase("double", "double")]
    [TestCase("short", "short")]
    [TestCase("int", "int")]
    [TestCase("long long", "long")]
    [TestCase("signed char", "sbyte")]
    [TestCase("float", "float")]
    [TestCase("unsigned short", "ushort")]
    [TestCase("unsigned int", "uint")]
    [TestCase("unsigned long long", "ulong")]
    [TestCase("bool", "byte")]
    public Task FixedSizedBufferPrimitiveTypedefTest(string nativeType, string expectedManagedType) => FixedSizedBufferPrimitiveTypedefTestImpl(nativeType, expectedManagedType);

    [Test]
    public Task GuidTest() => GuidTestImpl();

    [Test]
    public Task InheritanceTest() => InheritanceTestImpl();

    [Test]
    public Task InheritanceWithNativeInheritanceAttributeTest() => InheritanceWithNativeInheritanceAttributeTestImpl();

    [TestCase("double", "double", 10, 5)]
    [TestCase("short", "short", 10, 5)]
    [TestCase("int", "int", 10, 5)]
    [TestCase("float", "float", 10, 5)]
    public Task NestedAnonymousTest(string nativeType, string expectedManagedType, int line, int column) => NestedAnonymousTestImpl(nativeType, expectedManagedType, line, column);

    [Test]
    public Task NestedAnonymousWithBitfieldTest() => NestedAnonymousWithBitfieldTestImpl();

    [TestCase("double", "double")]
    [TestCase("short", "short")]
    [TestCase("int", "int")]
    [TestCase("float", "float")]
    public Task NestedTest(string nativeType, string expectedManagedType) => NestedTestImpl(nativeType, expectedManagedType);

    [TestCase("unsigned char", "byte")]
    [TestCase("long long", "long")]
    [TestCase("signed char", "sbyte")]
    [TestCase("unsigned short", "ushort")]
    [TestCase("unsigned int", "uint")]
    [TestCase("unsigned long long", "ulong")]
    [TestCase("bool", "byte")]
    public Task NestedWithNativeTypeNameTest(string nativeType, string expectedManagedType) => NestedWithNativeTypeNameTestImpl(nativeType, expectedManagedType);

    [Test]
    public Task NewKeywordTest() => NewKeywordTestImpl();

    [Test]
    public Task NoDefinitionTest() => NoDefinitionTestImpl();

    [Test]
    public Task PackTest() => PackTestImpl();

    [Test]
    public Task PointerToSelfTest() => PointerToSelfTestImpl();

    [Test]
    public Task PointerToSelfViaTypedefTest() => PointerToSelfViaTypedefTestImpl();

    [Test]
    public Task RemapTest() => RemapTestImpl();

    [Test]
    public Task RemapNestedAnonymousTest() => RemapNestedAnonymousTestImpl();

    [TestCase("double", "double")]
    [TestCase("short", "short")]
    [TestCase("int", "int")]
    [TestCase("float", "float")]
    public Task SkipNonDefinitionTest(string nativeType, string expectedManagedType) => SkipNonDefinitionTestImpl(nativeType, expectedManagedType);

    [Test]
    public Task SkipNonDefinitionPointerTest() => SkipNonDefinitionPointerTestImpl();

    [TestCase("unsigned char", "byte")]
    [TestCase("long long", "long")]
    [TestCase("signed char", "sbyte")]
    [TestCase("unsigned short", "ushort")]
    [TestCase("unsigned int", "uint")]
    [TestCase("unsigned long long", "ulong")]
    [TestCase("bool", "byte")]
    public Task SkipNonDefinitionWithNativeTypeNameTest(string nativeType, string expectedManagedType) => SkipNonDefinitionWithNativeTypeNameTestImpl(nativeType, expectedManagedType);

    [TestCase("unsigned char", "byte")]
    [TestCase("double", "double")]
    [TestCase("short", "short")]
    [TestCase("int", "int")]
    [TestCase("long long", "long")]
    [TestCase("signed char", "sbyte")]
    [TestCase("float", "float")]
    [TestCase("unsigned short", "ushort")]
    [TestCase("unsigned int", "uint")]
    [TestCase("unsigned long long", "ulong")]
    [TestCase("bool", "byte")]
    public Task TypedefTest(string nativeType, string expectedManagedType) => TypedefTestImpl(nativeType, expectedManagedType);

    [Test]
    public Task UsingDeclarationTest() => UsingDeclarationTestImpl();

    [Test]
    public Task WithAccessSpecifierTest() => WithAccessSpecifierTestImpl();

    [Test]
    public Task SourceLocationAttributeTest() => SourceLocationAttributeTestImpl();

    protected abstract Task IncompleteArraySizeTestImpl(string nativeType, string expectedManagedType);

    protected abstract Task BasicTestImpl(string nativeType, string expectedManagedType);

    protected abstract Task BasicTestInCModeImpl(string nativeType, string expectedManagedType);

    protected abstract Task BasicWithNativeTypeNameTestImpl(string nativeType, string expectedManagedType);

    protected abstract Task BitfieldTestImpl();

    protected abstract Task ExcludeTestImpl();

    protected abstract Task FixedSizedBufferNonPrimitiveTestImpl(string nativeType, string expectedManagedType);

    protected abstract Task FixedSizedBufferNonPrimitiveMultidimensionalTestImpl(string nativeType, string expectedManagedType);

    protected abstract Task FixedSizedBufferNonPrimitiveTypedefTestImpl(string nativeType, string expectedManagedType);

    protected abstract Task FixedSizedBufferNonPrimitiveWithNativeTypeNameTestImpl(string nativeType, string expectedManagedType);

    protected abstract Task FixedSizedBufferPointerTestImpl(string nativeType, string expectedManagedType);

    protected abstract Task FixedSizedBufferPrimitiveTestImpl(string nativeType, string expectedManagedType);

    protected abstract Task FixedSizedBufferPrimitiveMultidimensionalTestImpl(string nativeType, string expectedManagedType);

    protected abstract Task FixedSizedBufferPrimitiveTypedefTestImpl(string nativeType, string expectedManagedType);

    protected abstract Task GuidTestImpl();

    protected abstract Task InheritanceTestImpl();

    protected abstract Task InheritanceWithNativeInheritanceAttributeTestImpl();

    protected abstract Task NestedAnonymousTestImpl(string nativeType, string expectedManagedType, int line, int column);

    protected abstract Task NestedAnonymousWithBitfieldTestImpl();

    protected abstract Task NestedTestImpl(string nativeType, string expectedManagedType);

    protected abstract Task NestedWithNativeTypeNameTestImpl(string nativeType, string expectedManagedType);

    protected abstract Task NewKeywordTestImpl();

    protected abstract Task NoDefinitionTestImpl();

    protected abstract Task PackTestImpl();

    protected abstract Task PointerToSelfTestImpl();

    protected abstract Task PointerToSelfViaTypedefTestImpl();

    protected abstract Task RemapTestImpl();

    protected abstract Task RemapNestedAnonymousTestImpl();

    protected abstract Task SkipNonDefinitionTestImpl(string nativeType, string expectedManagedType);

    protected abstract Task SkipNonDefinitionPointerTestImpl();

    protected abstract Task SkipNonDefinitionWithNativeTypeNameTestImpl(string nativeType, string expectedManagedType);

    protected abstract Task TypedefTestImpl(string nativeType, string expectedManagedType);

    protected abstract Task UsingDeclarationTestImpl();

    protected abstract Task WithAccessSpecifierTestImpl();

    protected abstract Task SourceLocationAttributeTestImpl();
}
