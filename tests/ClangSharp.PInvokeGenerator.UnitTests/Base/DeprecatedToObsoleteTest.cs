// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

public abstract class DeprecatedToObsoleteTest : PInvokeGeneratorTest
{
    [TestCase("int", "int")]
    public Task SimpleStructMembers(string nativeType, string expectedManagedType) => SimpleStructMembersImpl(nativeType, expectedManagedType);

    [Test]
    public Task StructDecl() => StructDeclImpl();

    [TestCase("int", "int")]
    public Task SimpleTypedefStructMembers(string nativeType, string expectedManagedType) => SimpleTypedefStructMembersImpl(nativeType, expectedManagedType);

    [Test]
    public Task TypedefStructDecl() => TypedefStructDeclImpl();

    [Test]
    public Task SimpleEnumMembers() => SimpleEnumMembersImpl();

    [Test]
    public Task EnumDecl() => EnumDeclImpl();

    [TestCase("int", "int")]
    public Task SimpleVarDecl(string nativeType, string expectedManagedType) => SimpleVarDeclImpl(nativeType, expectedManagedType);

    protected abstract Task SimpleStructMembersImpl(string nativeType, string expectedManagedType);

    protected abstract Task StructDeclImpl();

    protected abstract Task SimpleTypedefStructMembersImpl(string nativeType, string expectedManagedType);

    protected abstract Task TypedefStructDeclImpl();

    protected abstract Task SimpleEnumMembersImpl();

    protected abstract Task EnumDeclImpl();

    protected abstract Task SimpleVarDeclImpl(string nativeType, string expectedManagedType);
}
