﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ClangSharp.Test
{
    public sealed class FunctionDeclarationBodyImportTest : PInvokeGeneratorTest
    {
        [Fact]
        public async Task BasicTest()
        {
            var inputContents = @"void MyFunction()
{
}
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public static partial class Methods
    {
        private const string libraryPath = ""ClangSharpPInvokeGenerator"";

        public static void MyFunction()
        {
        }
    }
}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

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
        [InlineData("==")]
        [InlineData(">>")]
        [InlineData(">>=")]
        [InlineData("^")]
        [InlineData("^=")]
        [InlineData("|")]
        [InlineData("|=")]
        public async Task BinaryOperatorBasicTest(string opcode)
        {
            var inputContents = $@"int MyFunction(int x, int y)
{{
    return x {opcode} y;
}}
";

            var expectedOutputContents = $@"namespace ClangSharp.Test
{{
    public static partial class Methods
    {{
        private const string libraryPath = ""ClangSharpPInvokeGenerator"";

        public static int MyFunction(int x, int y)
        {{
            return x {opcode} y;
        }}
    }}
}}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Theory]
        [InlineData("!=")]
        [InlineData("&&")]
        [InlineData("<")]
        [InlineData("<=")]
        [InlineData(">")]
        [InlineData(">=")]
        [InlineData("||")]
        public async Task BinaryOperatorBooleanTest(string opcode)
        {
            var inputContents = $@"bool MyFunction(bool x, bool y)
{{
    return x {opcode} y;
}}
";

            var expectedOutputContents = $@"namespace ClangSharp.Test
{{
    public static partial class Methods
    {{
        private const string libraryPath = ""ClangSharpPInvokeGenerator"";

        public static bool MyFunction(bool x, bool y)
        {{
            return x {opcode} y;
        }}
    }}
}}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task CallFunctionTest()
        {
            var inputContents = @"void MyCalledFunction()
{
}

void MyFunction()
{
    MyCalledFunction();
}
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public static partial class Methods
    {
        private const string libraryPath = ""ClangSharpPInvokeGenerator"";

        public static void MyCalledFunction()
        {
        }

        public static void MyFunction()
        {
            MyCalledFunction();
        }
    }
}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task ReturnIntegerTest()
        {
            var inputContents = @"int MyFunction()
{
    return -1;
}
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public static partial class Methods
    {
        private const string libraryPath = ""ClangSharpPInvokeGenerator"";

        public static int MyFunction()
        {
            return -1;
        }
    }
}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task UnaryOperatorAddrOfUnsafeTest()
        {
            var inputContents = @"int* MyFunction(int value)
{
    return &value;
}
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public static unsafe partial class Methods
    {
        private const string libraryPath = ""ClangSharpPInvokeGenerator"";

        public static int* MyFunction(int value)
        {
            return &value;
        }
    }
}
";

            await ValidateUnsafeGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task UnaryOperatorDerefUnsafeTest()
        {
            var inputContents = @"int MyFunction(int* value)
{
    return *value;
}
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public static unsafe partial class Methods
    {
        private const string libraryPath = ""ClangSharpPInvokeGenerator"";

        public static int MyFunction(int* value)
        {
            return *value;
        }
    }
}
";

            await ValidateUnsafeGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task UnaryOperatorLogicalNotTest()
        {
            var inputContents = @"bool MyFunction(bool value)
{
    return !value;
}
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public static partial class Methods
    {
        private const string libraryPath = ""ClangSharpPInvokeGenerator"";

        public static bool MyFunction(bool value)
        {
            return !value;
        }
    }
}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Theory]
        [InlineData("++")]
        [InlineData("--")]
        public async Task UnaryOperatorPostfixTest(string opcode)
        {
            var inputContents = $@"int MyFunction(int value)
{{
    return value{opcode};
}}
";

            var expectedOutputContents = $@"namespace ClangSharp.Test
{{
    public static partial class Methods
    {{
        private const string libraryPath = ""ClangSharpPInvokeGenerator"";

        public static int MyFunction(int value)
        {{
            return value{opcode};
        }}
    }}
}}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Theory]
        [InlineData("+")]
        [InlineData("++")]
        [InlineData("-")]
        [InlineData("--")]
        [InlineData("~")]
        public async Task UnaryOperatorPrefixTest(string opcode)
        {
            var inputContents = $@"int MyFunction(int value)
{{
    return {opcode}value;
}}
";

            var expectedOutputContents = $@"namespace ClangSharp.Test
{{
    public static partial class Methods
    {{
        private const string libraryPath = ""ClangSharpPInvokeGenerator"";

        public static int MyFunction(int value)
        {{
            return {opcode}value;
        }}
    }}
}}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }
    }
}
