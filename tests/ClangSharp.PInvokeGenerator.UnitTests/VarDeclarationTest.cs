// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ClangSharp.UnitTests
{
    public sealed class VarDeclarationTest : PInvokeGeneratorTest
    {
        [Theory]
        [InlineData("double", "double")]
        [InlineData("short", "short")]
        [InlineData("int", "int")]
        [InlineData("float", "float")]
        public async Task BasicTest(string nativeType, string expectedManagedType)
        {
            var inputContents = $@"{nativeType} MyVariable = 0;";

            var expectedOutputContents = $@"namespace ClangSharp.Test
{{
    public static partial class Methods
    {{
        public static {expectedManagedType} MyVariable = 0;
    }}
}}
";

            await ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents);
        }

        [Theory]
        [InlineData("unsigned char", "byte")]
        [InlineData("long long", "long")]
        [InlineData("signed char", "sbyte")]
        [InlineData("unsigned short", "ushort")]
        [InlineData("unsigned int", "uint")]
        [InlineData("unsigned long long", "ulong")]
        public async Task BasicWithNativeTypeNameTest(string nativeType, string expectedManagedType)
        {
            var inputContents = $@"{nativeType} MyVariable = 0;";

            var expectedOutputContents = $@"namespace ClangSharp.Test
{{
    public static partial class Methods
    {{
        [NativeTypeName(""{nativeType}"")]
        public static {expectedManagedType} MyVariable = 0;
    }}
}}
";

            await ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task GuidMacroTest()
        {
            var inputContents = $@"struct GUID {{
    unsigned long  Data1;
    unsigned short Data2;
    unsigned short Data3;
    unsigned char  Data4[8];
}};

const GUID IID_IUnknown = {{ 0x00000000, 0x0000, 0x0000, {{ 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46 }} }};
";

            var expectedOutputContents = $@"using System;

namespace ClangSharp.Test
{{
    public static partial class Methods
    {{
        [NativeTypeName(""const GUID"")]
        public static readonly Guid IID_IUnknown = new Guid(0x00000000, 0x0000, 0x0000, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46);
    }}
}}
";
            var excludedNames = new string[] { "GUID" };
            var remappedNames = new Dictionary<string, string> { ["GUID"] = "Guid" };

            await ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, excludedNames: excludedNames, remappedNames: remappedNames);
        }

        [Theory]
        [InlineData("0", "int", "0")]
        [InlineData("0U", "uint", "0U")]
        [InlineData("0LL", "long", "0L")]
        [InlineData("0ULL", "ulong", "0UL")]
        [InlineData("0.0", "double", "0.0")]
        [InlineData("0.f", "float", "0.0f")]
        public async Task MacroTest(string nativeValue, string expectedManagedType, string expectedManagedValue)
        {
            var inputContents = $@"#define MyMacro1 {nativeValue}
#define MyMacro2 MyMacro1";

            var expectedOutputContents = $@"namespace ClangSharp.Test
{{
    public static partial class Methods
    {{
        [NativeTypeName(""#define MyMacro1 {nativeValue}"")]
        public const {expectedManagedType} MyMacro1 = {expectedManagedValue};

        [NativeTypeName(""#define MyMacro2 MyMacro1"")]
        public const {expectedManagedType} MyMacro2 = {expectedManagedValue};
    }}
}}
";

            await ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task MultilineMacroTest()
        {
            var inputContents = $@"#define MyMacro1 0 + \
1";

            var expectedOutputContents = $@"namespace ClangSharp.Test
{{
    public static partial class Methods
    {{
        [NativeTypeName(""#define MyMacro1 0 + \\\n1"")]
        public const int MyMacro1 = 0 + 1;
    }}
}}
";

            await ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents);
        }

        [Theory]
        [InlineData("double")]
        [InlineData("short")]
        [InlineData("int")]
        [InlineData("float")]
        public async Task NoInitializerTest(string nativeType)
        {
            var inputContents = $@"{nativeType} MyVariable;";
            var expectedOutputContents = "";
            await ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task Utf8StringLiteralMacroTest()
        {
            var inputContents = $@"#define MyMacro1 ""Test""";

            var expectedOutputContents = $@"using System;

namespace ClangSharp.Test
{{
    public static partial class Methods
    {{
        [NativeTypeName(""#define MyMacro1 \""Test\"""")]
        public static ReadOnlySpan<byte> MyMacro1 => new byte[] {{ 0x54, 0x65, 0x73, 0x74, 0x00 }};
    }}
}}
";

            await ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task Utf16StringLiteralMacroTest()
        {
            var inputContents = $@"#define MyMacro1 u""Test""";

            var expectedOutputContents = $@"namespace ClangSharp.Test
{{
    public static partial class Methods
    {{
        [NativeTypeName(""#define MyMacro1 u\""Test\"""")]
        public const string MyMacro1 = ""Test"";
    }}
}}
";

            await ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task UncheckedConversionMacroTest()
        {
            var inputContents = $@"#define MyMacro1 (long)0x80000000L
#define MyMacro2 (int)0x80000000";

            var expectedOutputContents = $@"namespace ClangSharp.Test
{{
    public static partial class Methods
    {{
        [NativeTypeName(""#define MyMacro1 (long)0x80000000L"")]
        public const int MyMacro1 = unchecked((int)(0x80000000));

        [NativeTypeName(""#define MyMacro2 (int)0x80000000"")]
        public const int MyMacro2 = unchecked((int)(0x80000000));
    }}
}}
";

            await ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task UncheckedConversionMacroTest2()
        {
            var inputContents = $@"#define MyMacro1(x, y, z) ((int)(((unsigned long)(x)<<31) | ((unsigned long)(y)<<16) | ((unsigned long)(z))))
#define MyMacro2(n) MyMacro1(1, 2, n)
#define MyMacro3 MyMacro2(3)";

            var expectedOutputContents = $@"namespace ClangSharp.Test
{{
    public static partial class Methods
    {{
        [NativeTypeName(""#define MyMacro3 MyMacro2(3)"")]
        public const int MyMacro3 = unchecked((int)(((uint)(1) << 31) | ((uint)(2) << 16) | ((uint)(3))));
    }}
}}
";

            var excludedNames = new string[] { "MyMacro1", "MyMacro2" };
            await ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, excludedNames: excludedNames);
        }

        [Fact]
        public async Task UncheckedPointerMacroTest()
        {
            var inputContents = $@"#define Macro1 ((int*) -1)";

            var expectedOutputContents = $@"namespace ClangSharp.Test
{{
    public static unsafe partial class Methods
    {{
        [NativeTypeName(""#define Macro1 ((int*) -1)"")]
        public static readonly int* Macro1 = unchecked((int*)(-1));
    }}
}}
";

            await ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents);
        }

    }
}
