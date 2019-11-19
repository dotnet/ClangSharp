// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Threading.Tasks;
using Xunit;

namespace ClangSharp.UnitTests
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
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

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
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

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
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

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
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

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
        public async Task CallFunctionWithArgsTest()
        {
            var inputContents = @"void MyCalledFunction(int x, int y)
{
}

void MyFunction()
{
    MyCalledFunction(0, 1);
}
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public static partial class Methods
    {
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

        public static void MyCalledFunction(int x, int y)
        {
        }

        public static void MyFunction()
        {
            MyCalledFunction(0, 1);
        }
    }
}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task CompareMultipleEnumTest()
        {
            var inputContents = @"enum MyEnum : int
{
    MyEnum_Value0,
    MyEnum_Value1,
    MyEnum_Value2,
};

static inline int MyFunction(MyEnum x)
{
    return x == MyEnum_Value0 ||
           x == MyEnum_Value1 ||
           x == MyEnum_Value2;
}
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public enum MyEnum
    {
        MyEnum_Value0,
        MyEnum_Value1,
        MyEnum_Value2,
    }

    public static partial class Methods
    {
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

        public static int MyFunction(MyEnum x)
        {
            return x == MyEnum_Value0 || x == MyEnum_Value1 || x == MyEnum_Value2;
        }
    }
}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task CStyleFunctionalCastTest()
        {
            var inputContents = @"int MyFunction(float input)
{
    return (int)input;
}
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public static partial class Methods
    {
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

        public static int MyFunction(float input)
        {
            return (int)input;
        }
    }
}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task CxxFunctionalCastTest()
        {
            var inputContents = @"int MyFunction(float input)
{
    return int(input);
}
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public static partial class Methods
    {
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

        public static int MyFunction(float input)
        {
            return (int)input;
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
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

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
        public async Task UnaryOperatorAddrOfTest()
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
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

        [return: NativeTypeName(""int *"")]
        public static int* MyFunction(int value)
        {
            return &value;
        }
    }
}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task UnaryOperatorDerefTest()
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
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

        public static int MyFunction([NativeTypeName(""int *"")] int* value)
        {
            return *value;
        }
    }
}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
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
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

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
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

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
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

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
