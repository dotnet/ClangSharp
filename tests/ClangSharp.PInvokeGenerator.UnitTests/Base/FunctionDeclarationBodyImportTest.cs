// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests
{
    public abstract class FunctionDeclarationBodyImportTest : PInvokeGeneratorTest
    {
        [Test]
        public Task AccessUnionMemberTest() => AccessUnionMemberTestImpl();

        [Test]
        public Task ArraySubscriptTest() => ArraySubscriptTestImpl();

        [Test]
        public Task BasicTest() => BasicTestImpl();

        [TestCase("%")]
        [TestCase("%=")]
        [TestCase("&")]
        [TestCase("&=")]
        [TestCase("*")]
        [TestCase("*=")]
        [TestCase("+")]
        [TestCase("+=")]
        [TestCase("-")]
        [TestCase("-=")]
        [TestCase("/")]
        [TestCase("/=")]
        [TestCase("<<")]
        [TestCase("<<=")]
        [TestCase("=")]
        [TestCase(">>")]
        [TestCase(">>=")]
        [TestCase("^")]
        [TestCase("^=")]
        [TestCase("|")]
        [TestCase("|=")]
        public Task BinaryOperatorBasicTest(string opcode) => BinaryOperatorBasicTestImpl(opcode);

        [TestCase("==")]
        [TestCase("!=")]
        [TestCase("<")]
        [TestCase("<=")]
        [TestCase(">")]
        [TestCase(">=")]
        public Task BinaryOperatorCompareTest(string opcode) => BinaryOperatorCompareTestImpl(opcode);

        [TestCase("&&")]
        [TestCase("||")]
        public Task BinaryOperatorBooleanTest(string opcode) => BinaryOperatorBooleanTestImpl(opcode);

        [Test]
        public Task BreakTest() => BreakTestImpl();

        [Test]
        public Task CallFunctionTest() => CallFunctionTestImpl();

        [Test]
        public Task CallFunctionWithArgsTest() => CallFunctionWithArgsTestImpl();

        [Test]
        public Task CaseTest() => CaseTestImpl();

        [Test]
        public Task CaseNoCompoundTest() => CaseNoCompoundTestImpl();

        [Test]
        public Task CompareMultipleEnumTest() => CompareMultipleEnumTestImpl();

        [Test]
        public Task ConditionalOperatorTest() => ConditionalOperatorTestImpl();

        [Test]
        public Task ContinueTest() => ContinueTestImpl();

        [Test]
        public Task CStyleFunctionalCastTest() => CStyleFunctionalCastTestImpl();

        [Test]
        public Task CxxFunctionalCastTest() => CxxFunctionalCastTestImpl();

        [Test]
        public Task CxxConstCastTest() => CxxConstCastTestImpl();

        [Test]
        public Task CxxDynamicCastTest() => CxxDynamicCastTestImpl();

        [Test]
        public Task CxxReinterpretCastTest() => CxxReinterpretCastTestImpl();

        [Test]
        public Task CxxStaticCastTest() => CxxStaticCastTestImpl();

        [Test]
        public Task DeclTest() => DeclTestImpl();

        [Test]
        public Task DoTest() => DoTestImpl();

        [Test]
        public Task DoNonCompoundTest() => DoNonCompoundTestImpl();

        [Test]
        public Task ForTest() => ForTestImpl();

        [Test]
        public Task ForNonCompoundTest() => ForNonCompoundTestImpl();

        [Test]
        public Task IfTest() => IfTestImpl();

        [Test]
        public Task IfElseTest() => IfElseTestImpl();

        [Test]
        public Task IfElseIfTest() => IfElseIfTestImpl();

        [Test]
        public Task IfElseNonCompoundTest() => IfElseNonCompoundTestImpl();

        [Test]
        public Task InitListForArrayTest() => InitListForArrayTestImpl();

        [Test]
        public Task InitListForRecordDeclTest() => InitListForRecordDeclTestImpl();

        [Test]
        public Task MemberTest() => MemberTestImpl();

        [Test]
        public Task RefToPtrTest() => RefToPtrTestImpl();

        [Test]
        public Task ReturnCXXNullPtrTest() => ReturnCXXNullPtrTestImpl();

        [TestCase("false")]
        [TestCase("true")]
        public Task ReturnCXXBooleanLiteralTest(string value) => ReturnCXXBooleanLiteralTestImpl(value);

        [TestCase("5e-1")]
        [TestCase("3.14")]
        public Task ReturnFloatingLiteralDoubleTest(string value) => ReturnFloatingLiteralDoubleTestImpl(value);

        [TestCase("5e-1f")]
        [TestCase("3.14f")]
        public Task ReturnFloatingLiteralSingleTest(string value) => ReturnFloatingLiteralSingleTestImpl(value);

        [Test]
        public Task ReturnEmptyTest() => ReturnEmptyTestImpl();

        [Test]
        public Task ReturnIntegerLiteralInt32Test() => ReturnIntegerLiteralInt32TestImpl();

        [Test]
        public Task ReturnStructTest() => ReturnStructTestImpl();

        [Test]
        public Task SwitchTest() => SwitchTestImpl();

        [Test]
        public Task SwitchNonCompoundTest() => SwitchNonCompoundTestImpl();

        [Test]
        public Task UnaryOperatorAddrOfTest() => UnaryOperatorAddrOfTestImpl();

        [Test]
        public Task UnaryOperatorDerefTest() => UnaryOperatorDerefTestImpl();

        [Test]
        public Task UnaryOperatorLogicalNotTest() => UnaryOperatorLogicalNotTestImpl();

        [TestCase("++")]
        [TestCase("--")]
        public Task UnaryOperatorPostfixTest(string opcode) => UnaryOperatorPostfixTestImpl(opcode);

        [TestCase("+")]
        [TestCase("++")]
        [TestCase("-")]
        [TestCase("--")]
        [TestCase("~")]
        public Task UnaryOperatorPrefixTest(string opcode) => UnaryOperatorPrefixTestImpl(opcode);

        [Test]
        public Task WhileTest() => WhileTestImpl();

        [Test]
        public Task WhileNonCompoundTest() => WhileNonCompoundTestImpl();

        protected abstract Task AccessUnionMemberTestImpl();

        protected abstract Task ArraySubscriptTestImpl();

        protected abstract Task BasicTestImpl();

        protected abstract Task BinaryOperatorBasicTestImpl(string opcode);

        protected abstract Task BinaryOperatorCompareTestImpl(string opcode);

        protected abstract Task BinaryOperatorBooleanTestImpl(string opcode);

        protected abstract Task BreakTestImpl();

        protected abstract Task CallFunctionTestImpl();

        protected abstract Task CallFunctionWithArgsTestImpl();

        protected abstract Task CaseTestImpl();

        protected abstract Task CaseNoCompoundTestImpl();

        protected abstract Task CompareMultipleEnumTestImpl();

        protected abstract Task ConditionalOperatorTestImpl();

        protected abstract Task ContinueTestImpl();

        protected abstract Task CStyleFunctionalCastTestImpl();

        protected abstract Task CxxFunctionalCastTestImpl();

        protected abstract Task CxxConstCastTestImpl();

        protected abstract Task CxxDynamicCastTestImpl();

        protected abstract Task CxxReinterpretCastTestImpl();

        protected abstract Task CxxStaticCastTestImpl();

        protected abstract Task DeclTestImpl();

        protected abstract Task DoTestImpl();

        protected abstract Task DoNonCompoundTestImpl();

        protected abstract Task ForTestImpl();

        protected abstract Task ForNonCompoundTestImpl();

        protected abstract Task IfTestImpl();

        protected abstract Task IfElseTestImpl();

        protected abstract Task IfElseIfTestImpl();

        protected abstract Task IfElseNonCompoundTestImpl();

        protected abstract Task InitListForArrayTestImpl();

        protected abstract Task InitListForRecordDeclTestImpl();

        protected abstract Task MemberTestImpl();

        protected abstract Task RefToPtrTestImpl();

        protected abstract Task ReturnCXXNullPtrTestImpl();

        protected abstract Task ReturnCXXBooleanLiteralTestImpl(string value);

        protected abstract Task ReturnFloatingLiteralDoubleTestImpl(string value);

        protected abstract Task ReturnFloatingLiteralSingleTestImpl(string value);

        protected abstract Task ReturnEmptyTestImpl();

        protected abstract Task ReturnIntegerLiteralInt32TestImpl();

        protected abstract Task ReturnStructTestImpl();

        protected abstract Task SwitchTestImpl();

        protected abstract Task SwitchNonCompoundTestImpl();

        protected abstract Task UnaryOperatorAddrOfTestImpl();

        protected abstract Task UnaryOperatorDerefTestImpl();

        protected abstract Task UnaryOperatorLogicalNotTestImpl();

        protected abstract Task UnaryOperatorPostfixTestImpl(string opcode);

        protected abstract Task UnaryOperatorPrefixTestImpl(string opcode);

        protected abstract Task WhileTestImpl();

        protected abstract Task WhileNonCompoundTestImpl();
    }
}
