// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClangSharp.UnitTests
{
    public sealed class CSharpLatestUnix_VarDeclarationTest : VarDeclarationTest
    {
        public override Task BasicTest(string nativeType, string expectedManagedType)
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

            return ValidateGeneratedCSharpLatestUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task BasicWithNativeTypeNameTest(string nativeType, string expectedManagedType)
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

            return ValidateGeneratedCSharpLatestUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task GuidMacroTest()
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

            return ValidateGeneratedCSharpLatestUnixBindingsAsync(inputContents, expectedOutputContents, excludedNames: excludedNames, remappedNames: remappedNames);
        }

        public override Task MacroTest(string nativeValue, string expectedManagedType, string expectedManagedValue)
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

            return ValidateGeneratedCSharpLatestUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task MultilineMacroTest()
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

            return ValidateGeneratedCSharpLatestUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task NoInitializerTest(string nativeType)
        {
            var inputContents = $@"{nativeType} MyVariable;";
            var expectedOutputContents = "";
            return ValidateGeneratedCSharpLatestUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task Utf8StringLiteralMacroTest()
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

            return ValidateGeneratedCSharpLatestUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task Utf16StringLiteralMacroTest()
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

            return ValidateGeneratedCSharpLatestUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task WideStringLiteralConstTest()
{
            // Unsupported string literal kind: 'CX_CLK_Wide'
            return Task.CompletedTask;
        }

        public override Task StringLiteralConstTest()
        {
            var inputContents = $@"const char MyConst1[] = ""Test"";";

            var expectedOutputContents = $@"using System;

namespace ClangSharp.Test
{{
    public static partial class Methods
    {{
        [NativeTypeName(""const char [5]"")]
        public static ReadOnlySpan<byte> MyConst1 => new byte[] {{ 0x54, 0x65, 0x73, 0x74, 0x00 }};
    }}
}}
";

            return ValidateGeneratedCSharpLatestUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task UncheckedConversionMacroTest()
        {
            var inputContents = $@"#define MyMacro1 (long)0x80000000L
#define MyMacro2 (int)0x80000000";

            var expectedOutputContents = $@"using System;

namespace ClangSharp.Test
{{
    public static partial class Methods
    {{
        [NativeTypeName(""#define MyMacro1 (long)0x80000000L"")]
        public const IntPtr MyMacro1 = (IntPtr)(0x80000000);

        [NativeTypeName(""#define MyMacro2 (int)0x80000000"")]
        public const int MyMacro2 = unchecked((int)(0x80000000));
    }}
}}
";

            return ValidateGeneratedCSharpLatestUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task UncheckedFunctionLikeCastMacroTest()
        {
            var inputContents = $@"#define MyMacro1 unsigned(-1)";

            var expectedOutputContents = $@"namespace ClangSharp.Test
{{
    public static partial class Methods
    {{
        [NativeTypeName(""#define MyMacro1 unsigned(-1)"")]
        public const uint MyMacro1 = unchecked((uint)(-1));
    }}
}}
";

            return ValidateGeneratedCSharpLatestUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task UncheckedConversionMacroTest2()
        {
            var inputContents = $@"#define MyMacro1(x, y, z) ((int)(((unsigned long)(x)<<31) | ((unsigned long)(y)<<16) | ((unsigned long)(z))))
#define MyMacro2(n) MyMacro1(1, 2, n)
#define MyMacro3 MyMacro2(3)";

            var expectedOutputContents = $@"using System;

namespace ClangSharp.Test
{{
    public static partial class Methods
    {{
        [NativeTypeName(""#define MyMacro3 MyMacro2(3)"")]
        public const int MyMacro3 = unchecked((int)(((UIntPtr)(1) << 31) | ((UIntPtr)(2) << 16) | ((UIntPtr)(3))));
    }}
}}
";

            var excludedNames = new string[] { "MyMacro1", "MyMacro2" };
            return ValidateGeneratedCSharpLatestUnixBindingsAsync(inputContents, expectedOutputContents, excludedNames: excludedNames);
        }

        public override Task UncheckedPointerMacroTest()
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

            return ValidateGeneratedCSharpLatestUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task UncheckedReinterpretCastMacroTest()
        {
            var inputContents = $@"#define Macro1 reinterpret_cast<int*>(-1)";

            var expectedOutputContents = $@"namespace ClangSharp.Test
{{
    public static unsafe partial class Methods
    {{
        [NativeTypeName(""#define Macro1 reinterpret_cast<int*>(-1)"")]
        public static readonly int* Macro1 = unchecked((int*)(-1));
    }}
}}
";

            return ValidateGeneratedCSharpLatestUnixBindingsAsync(inputContents, expectedOutputContents);
        }
    }
}
