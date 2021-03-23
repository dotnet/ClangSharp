// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Threading.Tasks;
using Xunit;

namespace ClangSharp.UnitTests
{
    public abstract class VarDeclarationTest : PInvokeGeneratorTest
    {
        [Theory]
        [InlineData("double", "double")]
        [InlineData("short", "short")]
        [InlineData("int", "int")]
        [InlineData("float", "float")]
        public abstract Task BasicTest(string nativeType, string expectedManagedType);

        [Theory]
        [InlineData("unsigned char", "byte")]
        [InlineData("long long", "long")]
        [InlineData("signed char", "sbyte")]
        [InlineData("unsigned short", "ushort")]
        [InlineData("unsigned int", "uint")]
        [InlineData("unsigned long long", "ulong")]
        public abstract Task BasicWithNativeTypeNameTest(string nativeType, string expectedManagedType);

        [Fact]
        public abstract Task GuidMacroTest();

        [Theory]
        [InlineData("0", "int", "0")]
        [InlineData("0U", "uint", "0U")]
        [InlineData("0LL", "long", "0L")]
        [InlineData("0ULL", "ulong", "0UL")]
        [InlineData("0.0", "double", "0.0")]
        [InlineData("0.f", "float", "0.0f")]
        public abstract Task MacroTest(string nativeValue, string expectedManagedType, string expectedManagedValue);

        [Fact]
        public abstract Task MultilineMacroTest();

        [Theory]
        [InlineData("double")]
        [InlineData("short")]
        [InlineData("int")]
        [InlineData("float")]
        public abstract Task NoInitializerTest(string nativeType);

        [Fact]
        public abstract Task Utf8StringLiteralMacroTest();

        [Fact]
        public abstract Task Utf16StringLiteralMacroTest();

        [Fact]
        public abstract Task WideStringLiteralConstTest();

        [Fact]
        public abstract Task StringLiteralConstTest();

        [Fact]
        public abstract Task UncheckedConversionMacroTest();

        [Fact]
        public abstract Task UncheckedFunctionLikeCastMacroTest();

        [Fact]
        public abstract Task UncheckedConversionMacroTest2();

        [Fact]
        public abstract Task UncheckedPointerMacroTest();

        [Fact]
        public abstract Task UncheckedReinterpretCastMacroTest();

        [Fact]
        public abstract Task MultidimensionlArrayTest();
    }
}
