// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Threading.Tasks;
using Xunit;

namespace ClangSharp.UnitTests
{
    public abstract class StructDeclarationTest : PInvokeGeneratorTest
    {
        [Theory]
        [InlineData("double", "double")]
        [InlineData("short", "short")]
        [InlineData("int", "int")]
        [InlineData("float", "float")]
        public abstract Task BasicTest(string nativeType, string expectedManagedType);

        [Theory]
        [InlineData("double", "double")]
        [InlineData("short", "short")]
        [InlineData("int", "int")]
        [InlineData("float", "float")]
        public abstract Task BasicTestInCMode(string nativeType, string expectedManagedType);

        [Theory]
        [InlineData("unsigned char", "byte")]
        [InlineData("long long", "long")]
        [InlineData("signed char", "sbyte")]
        [InlineData("unsigned short", "ushort")]
        [InlineData("unsigned int", "uint")]
        [InlineData("unsigned long long", "ulong")]
        public abstract Task BasicWithNativeTypeNameTest(string nativeType, string expectedManagedType);

        [Fact]
        public abstract Task BitfieldTest();

        [Fact]
        public abstract Task ExcludeTest();

        [Theory]
        [InlineData("double", "double")]
        [InlineData("short", "short")]
        [InlineData("int", "int")]
        [InlineData("float", "float")]
        public abstract Task FixedSizedBufferNonPrimitiveTest(string nativeType, string expectedManagedType);

        [Theory]
        [InlineData("double", "double")]
        [InlineData("short", "short")]
        [InlineData("int", "int")]
        [InlineData("float", "float")]
        public abstract Task FixedSizedBufferNonPrimitiveMultidimensionalTest(string nativeType, string expectedManagedType);

        [Theory]
        [InlineData("double", "double")]
        [InlineData("short", "short")]
        [InlineData("int", "int")]
        [InlineData("float", "float")]
        public abstract Task FixedSizedBufferNonPrimitiveTypedefTest(string nativeType, string expectedManagedType);

        [Theory]
        [InlineData("unsigned char", "byte")]
        [InlineData("long long", "long")]
        [InlineData("signed char", "sbyte")]
        [InlineData("unsigned short", "ushort")]
        [InlineData("unsigned int", "uint")]
        [InlineData("unsigned long long", "ulong")]
        public abstract Task FixedSizedBufferNonPrimitiveWithNativeTypeNameTest(string nativeType, string expectedManagedType);

        [Theory]
        [InlineData("double *", "double*")]
        [InlineData("short *", "short*")]
        [InlineData("int *", "int*")]
        [InlineData("float *", "float*")]
        public abstract Task FixedSizedBufferPointerTest(string nativeType, string expectedManagedType);

        [Theory]
        [InlineData("unsigned char", "byte")]
        [InlineData("double", "double")]
        [InlineData("short", "short")]
        [InlineData("int", "int")]
        [InlineData("long long", "long")]
        [InlineData("signed char", "sbyte")]
        [InlineData("float", "float")]
        [InlineData("unsigned short", "ushort")]
        [InlineData("unsigned int", "uint")]
        [InlineData("unsigned long long", "ulong")]
        public abstract Task FixedSizedBufferPrimitiveTest(string nativeType, string expectedManagedType);

        [Theory]
        [InlineData("unsigned char", "byte")]
        [InlineData("double", "double")]
        [InlineData("short", "short")]
        [InlineData("int", "int")]
        [InlineData("long long", "long")]
        [InlineData("signed char", "sbyte")]
        [InlineData("float", "float")]
        [InlineData("unsigned short", "ushort")]
        [InlineData("unsigned int", "uint")]
        [InlineData("unsigned long long", "ulong")]
        public abstract Task FixedSizedBufferPrimitiveMultidimensionalTest(string nativeType, string expectedManagedType);

        [Theory]
        [InlineData("unsigned char", "byte")]
        [InlineData("double", "double")]
        [InlineData("short", "short")]
        [InlineData("int", "int")]
        [InlineData("long long", "long")]
        [InlineData("signed char", "sbyte")]
        [InlineData("float", "float")]
        [InlineData("unsigned short", "ushort")]
        [InlineData("unsigned int", "uint")]
        [InlineData("unsigned long long", "ulong")]
        public abstract Task FixedSizedBufferPrimitiveTypedefTest(string nativeType, string expectedManagedType);

        [Fact]
        public abstract Task GuidTest();

        [Fact]
        public abstract Task InheritanceTest();

        [Fact]
        public abstract Task InheritanceWithNativeInheritanceAttributeTest();

        [Theory]
        [InlineData("double", "double", 10, 5)]
        [InlineData("short", "short", 10, 5)]
        [InlineData("int", "int", 10, 5)]
        [InlineData("float", "float", 10, 5)]
        public abstract Task NestedAnonymousTest(string nativeType, string expectedManagedType, int line, int column);

        [Fact]
        public abstract Task NestedAnonymousWithBitfieldTest();

        [Theory]
        [InlineData("double", "double")]
        [InlineData("short", "short")]
        [InlineData("int", "int")]
        [InlineData("float", "float")]
        public abstract Task NestedTest(string nativeType, string expectedManagedType);

        [Theory]
        [InlineData("unsigned char", "byte")]
        [InlineData("long long", "long")]
        [InlineData("signed char", "sbyte")]
        [InlineData("unsigned short", "ushort")]
        [InlineData("unsigned int", "uint")]
        [InlineData("unsigned long long", "ulong")]
        public abstract Task NestedWithNativeTypeNameTest(string nativeType, string expectedManagedType);

        [Fact]
        public abstract Task NewKeywordTest();

        [Fact]
        public abstract Task NoDefinitionTest();

        [Fact]
        public abstract Task PackTest();

        [Fact]
        public abstract Task PointerToSelfTest();

        [Fact]
        public abstract Task PointerToSelfViaTypedefTest();

        [Fact]
        public abstract Task RemapTest();

        [Fact]
        public abstract Task RemapNestedAnonymousTest();

        [Theory]
        [InlineData("double", "double")]
        [InlineData("short", "short")]
        [InlineData("int", "int")]
        [InlineData("float", "float")]
        public abstract Task SkipNonDefinitionTest(string nativeType, string expectedManagedType);

        [Fact]
        public abstract Task SkipNonDefinitionPointerTest();

        [Theory]
        [InlineData("unsigned char", "byte")]
        [InlineData("long long", "long")]
        [InlineData("signed char", "sbyte")]
        [InlineData("unsigned short", "ushort")]
        [InlineData("unsigned int", "uint")]
        [InlineData("unsigned long long", "ulong")]
        public abstract Task SkipNonDefinitionWithNativeTypeNameTest(string nativeType, string expectedManagedType);

        [Theory]
        [InlineData("unsigned char", "byte")]
        [InlineData("double", "double")]
        [InlineData("short", "short")]
        [InlineData("int", "int")]
        [InlineData("long long", "long")]
        [InlineData("signed char", "sbyte")]
        [InlineData("float", "float")]
        [InlineData("unsigned short", "ushort")]
        [InlineData("unsigned int", "uint")]
        [InlineData("unsigned long long", "ulong")]
        public abstract Task TypedefTest(string nativeType, string expectedManagedType);

        [Fact]
        public abstract Task UsingDeclarationTest();

        [Fact]
        public abstract Task SourceLocationAttributeTest();
    }
}
