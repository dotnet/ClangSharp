// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Threading.Tasks;
using Xunit;

namespace ClangSharp.UnitTests
{
    public abstract class EnumDeclarationTest : PInvokeGeneratorTest
    {
        [Fact]
        public abstract Task BasicTest();

        [Fact]
        public abstract Task BasicValueTest();

        [Fact]
        public abstract Task ExcludeTest();

        [Theory]
        [InlineData("short", "short")]
        public abstract Task ExplicitTypedTest(string nativeType, string expectedManagedType);

        [Theory]
        [InlineData("unsigned char", "byte")]
        [InlineData("long long", "long")]
        [InlineData("signed char", "sbyte")]
        [InlineData("unsigned short", "ushort")]
        [InlineData("unsigned int", "uint")]
        [InlineData("unsigned long long", "ulong")]
        public abstract Task ExplicitTypedWithNativeTypeNameTest(string nativeType, string expectedManagedType);

        [Fact]
        public abstract Task RemapTest();

        [Fact]
        public abstract Task WithAttributeTest();

        [Fact]
        public abstract Task WithAttributeStarTest();

        [Fact]
        public abstract Task WithAttributeStarPlusTest();

        [Fact]
        public abstract Task WithNamespaceTest();

        [Fact]
        public abstract Task WithNamespaceStarTest();

        [Fact]
        public abstract Task WithNamespaceStarPlusTest();

        [Fact]
        public abstract Task WithCastToEnumType();

        [Fact]
        public abstract Task WithMultipleEnumsTest();

        [Fact]
        public abstract Task WithImplicitConversionTest();

        [Fact]
        public abstract Task WithTypeTest();

        [Fact]
        public abstract Task WithTypeAndImplicitConversionTest();

        [Fact]
        public abstract Task WithTypeStarTest();

        [Fact]
        public abstract Task WithTypeStarOverrideTest();
    }
}
