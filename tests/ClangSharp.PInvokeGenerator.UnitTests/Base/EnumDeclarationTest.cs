// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

public abstract class EnumDeclarationTest : PInvokeGeneratorTest
{
    [Test]
    public Task BasicTest() => BasicTestImpl();

    [Test]
    public Task BasicValueTest() => BasicValueTestImpl();

    [Test]
    public Task ExcludeTest() => ExcludeTestImpl();

    [TestCase("short", "short")]
    public Task ExplicitTypedTest(string nativeType, string expectedManagedType) => ExplicitTypedTestImpl(nativeType, expectedManagedType);

    [TestCase("unsigned char", "byte")]
    [TestCase("long long", "long")]
    [TestCase("signed char", "sbyte")]
    [TestCase("unsigned short", "ushort")]
    [TestCase("unsigned int", "uint")]
    [TestCase("unsigned long long", "ulong")]
    public Task ExplicitTypedWithNativeTypeNameTest(string nativeType, string expectedManagedType) => ExplicitTypedWithNativeTypeNameTestImpl(nativeType, expectedManagedType);

    [Test]
    public Task RemapTest() => RemapTestImpl();

    [Test]
    public Task WithAttributeTest() => WithAttributeTestImpl();

    [Test]
    public Task WithNamespaceTest() => WithNamespaceTestImpl();

    [Test]
    public Task WithNamespaceStarTest() => WithNamespaceStarTestImpl();

    [Test]
    public Task WithNamespaceStarPlusTest() => WithNamespaceStarPlusTestImpl();

    [Test]
    public Task WithCastToEnumType() => WithCastToEnumTypeImpl();

    [Test]
    public Task WithMultipleEnumsTest() => WithMultipleEnumsTestImpl();

    [Test]
    public Task WithImplicitConversionTest() => WithImplicitConversionTestImpl();

    [Test]
    public Task WithTypeTest() => WithTypeTestImpl();

    [Test]
    public Task WithTypeAndImplicitConversionTest() => WithTypeAndImplicitConversionTestImpl();

    [Test]
    public Task WithTypeStarTest() => WithTypeStarTestImpl();

    [Test]
    public Task WithTypeStarOverrideTest() => WithTypeStarOverrideTestImpl();

    [Test]
    public Task WithAnonymousEnumTest() => WithAnonymousEnumTestImpl();

    [Test]
    public Task WithReferenceToAnonymousEnumEnumeratorTest() => WithReferenceToAnonymousEnumEnumeratorTestImpl();

    protected abstract Task BasicTestImpl();

    protected abstract Task BasicValueTestImpl();

    protected abstract Task ExcludeTestImpl();

    protected abstract Task ExplicitTypedTestImpl(string nativeType, string expectedManagedType);

    protected abstract Task ExplicitTypedWithNativeTypeNameTestImpl(string nativeType, string expectedManagedType);


    protected abstract Task RemapTestImpl();

    protected abstract Task WithAttributeTestImpl();

    protected abstract Task WithNamespaceTestImpl();

    protected abstract Task WithNamespaceStarTestImpl();

    protected abstract Task WithNamespaceStarPlusTestImpl();

    protected abstract Task WithCastToEnumTypeImpl();

    protected abstract Task WithMultipleEnumsTestImpl();

    protected abstract Task WithImplicitConversionTestImpl();

    protected abstract Task WithTypeTestImpl();

    protected abstract Task WithTypeAndImplicitConversionTestImpl();

    protected abstract Task WithTypeStarTestImpl();

    protected abstract Task WithTypeStarOverrideTestImpl();

    protected abstract Task WithAnonymousEnumTestImpl();

    protected abstract Task WithReferenceToAnonymousEnumEnumeratorTestImpl();
}
