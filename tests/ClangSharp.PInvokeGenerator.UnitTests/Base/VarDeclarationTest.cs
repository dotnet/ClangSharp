// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests
{
    public abstract class VarDeclarationTest : PInvokeGeneratorTest
    {
        [TestCase("double", "double")]
        [TestCase("short", "short")]
        [TestCase("int", "int")]
        [TestCase("float", "float")]
        public Task BasicTest(string nativeType, string expectedManagedType) => BasicTestImpl(nativeType, expectedManagedType);

        [TestCase("unsigned char", "byte")]
        [TestCase("long long", "long")]
        [TestCase("signed char", "sbyte")]
        [TestCase("unsigned short", "ushort")]
        [TestCase("unsigned int", "uint")]
        [TestCase("unsigned long long", "ulong")]
        public Task BasicWithNativeTypeNameTest(string nativeType, string expectedManagedType) => BasicWithNativeTypeNameTestImpl(nativeType, expectedManagedType);

        [Test]
        public Task GuidMacroTest() => GuidMacroTestImpl();

        [TestCase("0", "int", "0")]
        [TestCase("0U", "uint", "0U")]
        [TestCase("0LL", "long", "0L")]
        [TestCase("0ULL", "ulong", "0UL")]
        [TestCase("0.0", "double", "0.0")]
        [TestCase("0.f", "float", "0.0f")]
        public Task MacroTest(string nativeValue, string expectedManagedType, string expectedManagedValue) => MacroTestImpl(nativeValue, expectedManagedType, expectedManagedValue);

        [Test]
        public Task MultilineMacroTest() => MultilineMacroTestImpl();

        [TestCase("double")]
        [TestCase("short")]
        [TestCase("int")]
        [TestCase("float")]
        public Task NoInitializerTest(string nativeType) => NoInitializerTestImpl(nativeType);

        [Test]
        public Task Utf8StringLiteralMacroTest() => Utf8StringLiteralMacroTestImpl();

        [Test]
        public Task Utf16StringLiteralMacroTest() => Utf16StringLiteralMacroTestImpl();

        [Test]
        public Task WideStringLiteralConstTest() => WideStringLiteralConstTestImpl();

        [Test]
        public Task StringLiteralConstTest() => StringLiteralConstTestImpl();

        [Test]
        public Task WideStringLiteralStaticConstTest() => WideStringLiteralStaticConstTestImpl();

        [Test]
        public Task StringLiteralStaticConstTest() => StringLiteralStaticConstTestImpl();

        [Test]
        public Task UncheckedConversionMacroTest() => UncheckedConversionMacroTestImpl();

        [Test]
        public Task UncheckedFunctionLikeCastMacroTest() => UncheckedFunctionLikeCastMacroTestImpl();

        [Test]
        public Task UncheckedConversionMacroTest2() => UncheckedConversionMacroTest2Impl();

        [Test]
        public Task UncheckedPointerMacroTest() => UncheckedPointerMacroTestImpl();

        [Test]
        public Task UncheckedReinterpretCastMacroTest() => UncheckedReinterpretCastMacroTestImpl();

        [Test]
        public Task MultidimensionlArrayTest() => MultidimensionlArrayTestImpl();

        [Test]
        public Task ConditionalDefineConstTest() => ConditionalDefineConstTestImpl();

        protected abstract Task BasicTestImpl(string nativeType, string expectedManagedType);

        protected abstract Task BasicWithNativeTypeNameTestImpl(string nativeType, string expectedManagedType);

        protected abstract Task GuidMacroTestImpl();

        protected abstract Task MacroTestImpl(string nativeValue, string expectedManagedType, string expectedManagedValue);

        protected abstract Task MultilineMacroTestImpl();

        protected abstract Task NoInitializerTestImpl(string nativeType);

        protected abstract Task Utf8StringLiteralMacroTestImpl();

        protected abstract Task Utf16StringLiteralMacroTestImpl();

        protected abstract Task WideStringLiteralConstTestImpl();

        protected abstract Task StringLiteralConstTestImpl();

        protected abstract Task WideStringLiteralStaticConstTestImpl();

        protected abstract Task StringLiteralStaticConstTestImpl();

        protected abstract Task UncheckedConversionMacroTestImpl();

        protected abstract Task UncheckedFunctionLikeCastMacroTestImpl();

        protected abstract Task UncheckedConversionMacroTest2Impl();

        protected abstract Task UncheckedPointerMacroTestImpl();

        protected abstract Task UncheckedReinterpretCastMacroTestImpl();

        protected abstract Task MultidimensionlArrayTestImpl();

        protected abstract Task ConditionalDefineConstTestImpl();
    }
}
