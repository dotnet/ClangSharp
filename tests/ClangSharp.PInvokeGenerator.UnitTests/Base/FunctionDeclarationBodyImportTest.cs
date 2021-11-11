// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Threading.Tasks;
using Xunit;

namespace ClangSharp.UnitTests
{
    public abstract class FunctionDeclarationBodyImportTest : PInvokeGeneratorTest
    {
        [Fact]
        public abstract Task AccessUnionMemberTest();

        [Fact]
        public abstract Task ArraySubscriptTest();

        [Fact]
        public abstract Task BasicTest();

        [Theory]
        [InlineData("%")]
        [InlineData("%=")]
        [InlineData("&")]
        [InlineData("&=")]
        [InlineData("*")]
        [InlineData("*=")]
        [InlineData("+")]
        [InlineData("+=")]
        [InlineData("-")]
        [InlineData("-=")]
        [InlineData("/")]
        [InlineData("/=")]
        [InlineData("<<")]
        [InlineData("<<=")]
        [InlineData("=")]
        [InlineData(">>")]
        [InlineData(">>=")]
        [InlineData("^")]
        [InlineData("^=")]
        [InlineData("|")]
        [InlineData("|=")]
        public abstract Task BinaryOperatorBasicTest(string opcode);

        [Theory]
        [InlineData("==")]
        [InlineData("!=")]
        [InlineData("<")]
        [InlineData("<=")]
        [InlineData(">")]
        [InlineData(">=")]
        public abstract Task BinaryOperatorCompareTest(string opcode);

        [Theory]
        [InlineData("&&")]
        [InlineData("||")]
        public abstract Task BinaryOperatorBooleanTest(string opcode);

        [Fact]
        public abstract Task BreakTest();

        [Fact]
        public abstract Task CallFunctionTest();

        [Fact]
        public abstract Task CallFunctionWithArgsTest();

        [Fact]
        public abstract Task CaseTest();

        [Fact]
        public abstract Task CaseNoCompoundTest();

        [Fact]
        public abstract Task CompareMultipleEnumTest();

        [Fact]
        public abstract Task ConditionalOperatorTest();

        [Fact]
        public abstract Task ContinueTest();

        [Fact]
        public abstract Task CStyleFunctionalCastTest();

        [Fact]
        public abstract Task CxxFunctionalCastTest();

        [Fact]
        public abstract Task CxxConstCastTest();

        [Fact]
        public abstract Task CxxDynamicCastTest();

        [Fact]
        public abstract Task CxxReinterpretCastTest();

        [Fact]
        public abstract Task CxxStaticCastTest();

        [Fact]
        public abstract Task DeclTest();

        [Fact]
        public abstract Task DoTest();

        [Fact]
        public abstract Task DoNonCompoundTest();

        [Fact]
        public abstract Task ForTest();

        [Fact]
        public abstract Task ForNonCompoundTest();

        [Fact]
        public abstract Task IfTest();

        [Fact]
        public abstract Task IfElseTest();

        [Fact]
        public abstract Task IfElseIfTest();

        [Fact]
        public abstract Task IfElseNonCompoundTest();

        [Fact]
        public abstract Task InitListForArrayTest();

        [Fact]
        public abstract Task InitListForRecordDeclTest();

        [Fact]
        public abstract Task MemberTest();

        [Fact]
        public abstract Task RefToPtrTest();

        [Fact]
        public abstract Task ReturnCXXNullPtrTest();

        [Theory]
        [InlineData("false")]
        [InlineData("true")]
        public abstract Task ReturnCXXBooleanLiteralTest(string value);

        [Theory]
        [InlineData("5e-1")]
        [InlineData("3.14")]
        public abstract Task ReturnFloatingLiteralDoubleTest(string value);

        [Theory]
        [InlineData("5e-1f")]
        [InlineData("3.14f")]
        public abstract Task ReturnFloatingLiteralSingleTest(string value);

        [Fact]
        public abstract Task ReturnEmptyTest();

        [Fact]
        public abstract Task ReturnIntegerLiteralInt32Test();

        [Fact]
        public abstract Task ReturnStructTest();

        [Fact]
        public abstract Task SwitchTest();

        [Fact]
        public abstract Task SwitchNonCompoundTest();

        [Fact]
        public abstract Task UnaryOperatorAddrOfTest();

        [Fact]
        public abstract Task UnaryOperatorDerefTest();

        [Fact]
        public abstract Task UnaryOperatorLogicalNotTest();

        [Theory]
        [InlineData("++")]
        [InlineData("--")]
        public abstract Task UnaryOperatorPostfixTest(string opcode);

        [Theory]
        [InlineData("+")]
        [InlineData("++")]
        [InlineData("-")]
        [InlineData("--")]
        [InlineData("~")]
        public abstract Task UnaryOperatorPrefixTest(string opcode);

        [Fact]
        public abstract Task WhileTest();

        [Fact]
        public abstract Task WhileNonCompoundTest();
    }
}
