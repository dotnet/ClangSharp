// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

public abstract class FunctionDeclarationDllImportTest : PInvokeGeneratorTest
{
    protected static readonly string[] TemplateTestExcludedNames = ["MyTemplate"];

    [Test]
    public Task BasicTest() => BasicTestImpl();

    [Test]
    public Task ArrayParameterTest() => ArrayParameterTestImpl();

    [Test]
    public Task FunctionPointerParameterTest() => FunctionPointerParameterTestImpl();

    [Test]
    public Task NamespaceTest() => NamespaceTestImpl();

    [TestCase("int", false, "int", "")]
    [TestCase("bool", true, "byte", "")]
    [TestCase("float *", true, "IntPtr", "using System;\n")]
    [TestCase("void (*)(int)", true, "IntPtr", "using System;\n")]
    public Task TemplateParameterTest(string nativeType, bool expectedNativeTypeAttr, string expectedManagedType, string expectedUsingStatement) => TemplateParameterTestImpl(nativeType, expectedNativeTypeAttr, expectedManagedType, expectedUsingStatement);

    [Test]
    public Task TemplateMemberTest() => TemplateMemberTestImpl();


    [Test]
    public Task NoLibraryPathTest() => NoLibraryPathTestImpl();

    [Test]
    public Task WithLibraryPathTest() => WithLibraryPathTestImpl();

    [Test]
    public Task WithLibraryPathStarTest() => WithLibraryPathStarTestImpl();

    [TestCase("unsigned char", "0", true, "byte", "0")]
    [TestCase("double", "1.0", false, "double", "1.0")]
    [TestCase("short", "2", false, "short", "2")]
    [TestCase("int", "3", false, "int", "3")]
    [TestCase("long long", "4", true, "long", "4")]
    [TestCase("signed char", "5", true, "sbyte", "5")]
    [TestCase("float", "6.0f", false, "float", "6.0f")]
    [TestCase("unsigned short", "7", true, "ushort", "7")]
    [TestCase("unsigned int", "8", true, "uint", "8")]
    [TestCase("unsigned long long", "9", true, "ulong", "9")]
    [TestCase("unsigned short", "'A'", true, "ushort", "(byte)('A')")]
    public Task OptionalParameterTest(string nativeType, string nativeInit, bool expectedNativeTypeNameAttr, string expectedManagedType, string expectedManagedInit) => OptionalParameterTestImpl(nativeType, nativeInit, expectedNativeTypeNameAttr, expectedManagedType, expectedManagedInit);

    [TestCase("void *", "nullptr", "void*", "null")]
    [TestCase("void *", "0", "void*", "null")]
    public Task OptionalParameterUnsafeTest(string nativeType, string nativeInit, string expectedManagedType, string expectedManagedInit) => OptionalParameterUnsafeTestImpl(nativeType, nativeInit, expectedManagedType, expectedManagedInit);

    [Test]
    public Task WithCallConvTest() => WithCallConvTestImpl();

    [Test]
    public Task WithCallConvStarTest() => WithCallConvStarTestImpl();

    [Test]
    public Task WithCallConvStarOverrideTest() => WithCallConvStarOverrideTestImpl();

    [Test]
    public Task WithSetLastErrorTest() => WithSetLastErrorTestImpl();

    [Test]
    public Task WithSetLastErrorStarTest() => WithSetLastErrorStarTestImpl();

    [Test]
    public Task SourceLocationTest() => SourceLocationTestImpl();

    [Test]
    public Task VarargsTest() => VarargsTestImpl();

    [Test]
    public Task IntrinsicsTest() => IntrinsicsTestImpl();

    protected abstract Task BasicTestImpl();

    protected abstract Task ArrayParameterTestImpl();

    protected abstract Task FunctionPointerParameterTestImpl();

    protected abstract Task NamespaceTestImpl();

    protected abstract Task TemplateParameterTestImpl(string nativeType, bool expectedNativeTypeAttr, string expectedManagedType, string expectedUsingStatement);

    protected abstract Task TemplateMemberTestImpl();

    protected abstract Task NoLibraryPathTestImpl();

    protected abstract Task WithLibraryPathTestImpl();

    protected abstract Task WithLibraryPathStarTestImpl();

    protected abstract Task OptionalParameterTestImpl(string nativeType, string nativeInit, bool expectedNativeTypeNameAttr, string expectedManagedType, string expectedManagedInit);

    protected abstract Task OptionalParameterUnsafeTestImpl(string nativeType, string nativeInit, string expectedManagedType, string expectedManagedInit);

    protected abstract Task WithCallConvTestImpl();

    protected abstract Task WithCallConvStarTestImpl();

    protected abstract Task WithCallConvStarOverrideTestImpl();

    protected abstract Task WithSetLastErrorTestImpl();

    protected abstract Task WithSetLastErrorStarTestImpl();

    protected abstract Task SourceLocationTestImpl();

    protected abstract Task VarargsTestImpl();

    protected abstract Task IntrinsicsTestImpl();
}
