// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

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

        [Theory]
        [InlineData("MyTemplate<int>", @"MyTemplate<int>", "")]
        [InlineData("MyTemplate<bool>", @"[NativeTypeName(""MyTemplate<bool>"")] MyTemplate<byte>", "")]
        [InlineData("MyTemplate<float*>", @"[NativeTypeName(""MyTemplate<float *>"")] MyTemplate<IntPtr>", "using System;\n")]
        [InlineData("MyTemplate<void(*)(int)>", @"[NativeTypeName(""MyTemplate<void (*)(int)>"")] MyTemplate<IntPtr>", "using System;\n")]
        public abstract Task TemplateParameterTest(string nativeParameter, string expectedManagedParameter, string expectedUsingStatement);

        [Fact]
        public abstract Task TemplateMemberTest();


        [Fact]
        public abstract Task NoLibraryPathTest();

        [Fact]
        public abstract Task WithLibraryPathTest();

        [Fact]
        public abstract Task WithLibraryPathStarTest();

        [Theory]
        [InlineData("unsigned char value = 0", @"[NativeTypeName(""unsigned char"")] byte value = 0")]
        [InlineData("double value = 1.0", @"double value = 1.0")]
        [InlineData("short value = 2", @"short value = 2")]
        [InlineData("int value = 3", @"int value = 3")]
        [InlineData("long long value = 4", @"[NativeTypeName(""long long"")] long value = 4")]
        [InlineData("signed char value = 5", @"[NativeTypeName(""signed char"")] sbyte value = 5")]
        [InlineData("float value = 6.0f", @"float value = 6.0f")]
        [InlineData("unsigned short value = 7", @"[NativeTypeName(""unsigned short"")] ushort value = 7")]
        [InlineData("unsigned int value = 8", @"[NativeTypeName(""unsigned int"")] uint value = 8")]
        [InlineData("unsigned long long value = 9", @"[NativeTypeName(""unsigned long long"")] ulong value = 9")]
        [InlineData("unsigned short value = 'A'", @"[NativeTypeName(""unsigned short"")] ushort value = (byte)('A')")]
        public abstract Task OptionalParameterTest(string nativeParameters, string expectedManagedParameters);

        [Theory]
        [InlineData("void* value = nullptr", @"[NativeTypeName(""void *"")] void* value = null")]
        [InlineData("void* value = 0", @"[NativeTypeName(""void *"")] void* value = null")]
        public abstract Task OptionalParameterUnsafeTest(string nativeParameters, string expectedManagedParameters);

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
    }
}
