// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Threading.Tasks;
using Xunit;

namespace ClangSharp.UnitTests
{
    public abstract class FunctionDeclarationDllImportTest : PInvokeGeneratorTest
    {
        [Fact]
        public abstract Task BasicTest();

        [Fact]
        public abstract Task ArrayParameterTest();

        [Fact]
        public abstract Task FunctionPointerParameterTest();

        [Fact]
        public abstract Task NamespaceTest();

        [Theory]
        [InlineData("int", false, "int", "")]
        [InlineData("bool", true, "byte", "")]
        [InlineData("float *", true, "IntPtr", "using System;\n")]
        [InlineData("void (*)(int)", true, "IntPtr", "using System;\n")]
        public abstract Task TemplateParameterTest(string nativeType, bool expectedNativeTypeAttr, string expectedManagedType, string expectedUsingStatement);

        [Fact]
        public abstract Task TemplateMemberTest();


        [Fact]
        public abstract Task NoLibraryPathTest();

        [Fact]
        public abstract Task WithLibraryPathTest();

        [Fact]
        public abstract Task WithLibraryPathStarTest();

        [Theory]
        [InlineData("unsigned char", "0", true, "byte", "0")]
        [InlineData("double", "1.0", false, "double", "1.0")]
        [InlineData("short", "2", false, "short", "2")]
        [InlineData("int", "3", false, "int", "3")]
        [InlineData("long long", "4", true, "long", "4")]
        [InlineData("signed char", "5", true, "sbyte", "5")]
        [InlineData("float", "6.0f", false, "float", "6.0f")]
        [InlineData("unsigned short", "7", true, "ushort", "7")]
        [InlineData("unsigned int", "8", true, "uint", "8")]
        [InlineData("unsigned long long", "9", true, "ulong", "9")]
        [InlineData("unsigned short", "'A'", true, "ushort", "(byte)('A')")]
        public abstract Task OptionalParameterTest(string nativeType, string nativeInit, bool expectedNativeTypeNameAttr, string expectedManagedType, string expectedManagedInit);

        [Theory]
        [InlineData("void *", "nullptr", "void*", "null")]
        [InlineData("void *", "0", "void*", "null")]
        public abstract Task OptionalParameterUnsafeTest(string nativeType, string nativeInit, string expectedManagedType, string expectedManagedInit);

        [Fact]
        public abstract Task WithCallConvTest();

        [Fact]
        public abstract Task WithCallConvStarTest();

        [Fact]
        public abstract Task WithCallConvStarOverrideTest();

        [Fact]
        public abstract Task WithSetLastErrorTest();

        [Fact]
        public abstract Task WithSetLastErrorStarTest();

        [Fact]
        public abstract Task SourceLocationTest();
    }
}
