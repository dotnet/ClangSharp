// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClangSharp.UnitTests
{
    public sealed class CSharpLatestWindows_VarDeclarationTest : VarDeclarationTest
    {
        protected override Task BasicTestImpl(string nativeType, string expectedManagedType)
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

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        protected override Task BasicWithNativeTypeNameTestImpl(string nativeType, string expectedManagedType)
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

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        protected override Task GuidMacroTestImpl()
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

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents, excludedNames: excludedNames, remappedNames: remappedNames);
        }

        protected override Task MacroTestImpl(string nativeValue, string expectedManagedType, string expectedManagedValue)
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

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        protected override Task MultilineMacroTestImpl()
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

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        protected override Task NoInitializerTestImpl(string nativeType)
        {
            var inputContents = $@"{nativeType} MyVariable;";
            var expectedOutputContents = "";
            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        protected override Task Utf8StringLiteralMacroTestImpl()
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

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        protected override Task Utf16StringLiteralMacroTestImpl()
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

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        protected override Task WideStringLiteralConstTestImpl()
        {
            var inputContents = $@"const wchar_t MyConst1[] = L""Test"";";

            var expectedOutputContents = $@"namespace ClangSharp.Test
{{
    public static partial class Methods
    {{
        [NativeTypeName(""const wchar_t[5]"")]
        public const string MyConst1 = ""Test"";
    }}
}}
";

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        protected override Task StringLiteralConstTestImpl()
        {
            var inputContents = $@"const char MyConst1[] = ""Test"";";

            var expectedOutputContents = $@"using System;

namespace ClangSharp.Test
{{
    public static partial class Methods
    {{
        [NativeTypeName(""const char[5]"")]
        public static ReadOnlySpan<byte> MyConst1 => new byte[] {{ 0x54, 0x65, 0x73, 0x74, 0x00 }};
    }}
}}
";

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        protected override Task UncheckedConversionMacroTestImpl()
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

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        protected override Task UncheckedFunctionLikeCastMacroTestImpl()
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

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        protected override Task UncheckedConversionMacroTest2Impl()
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
            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents, excludedNames: excludedNames);
        }

        protected override Task UncheckedPointerMacroTestImpl()
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

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        protected override Task UncheckedReinterpretCastMacroTestImpl()
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

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        protected override Task MultidimensionlArrayTestImpl()
        {
            var inputContents = $@"const int MyArray[2][2] = {{ {{ 0, 1 }}, {{ 2, 3 }} }};";

            var expectedOutputContents = $@"namespace ClangSharp.Test
{{
    public static partial class Methods
    {{
        [NativeTypeName(""const int[2][2]"")]
        public static readonly int[][] MyArray = new int[2][]
        {{
            new int[2]
            {{
                0,
                1,
            }},
            new int[2]
            {{
                2,
                3,
            }},
        }};
    }}
}}
";

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        protected override Task ConditionalDefineConstTestImpl()
        {
            var inputContents = @"typedef int TESTRESULT;
#define TESTRESULT_FROM_WIN32(x) ((TESTRESULT)(x) <= 0 ? ((TESTRESULT)(x)) : ((TESTRESULT) (((x) & 0x0000FFFF) | (7 << 16) | 0x80000000)))
#define ADDRESS_IN_USE TESTRESULT_FROM_WIN32(10048)";

            var expectedOutputContents = $@"namespace ClangSharp.Test
{{
    public static partial class Methods
    {{
        [NativeTypeName(""#define ADDRESS_IN_USE TESTRESULT_FROM_WIN32(10048)"")]
        public const int ADDRESS_IN_USE = unchecked((int)(10048) <= 0 ? ((int)(10048)) : ((int)(((10048) & 0x0000FFFF) | (7 << 16) | 0x80000000)));
    }}
}}
";
            var diagnostics = new Diagnostic[] { new Diagnostic(DiagnosticLevel.Warning, "Function like macro definition records are not supported: 'TESTRESULT_FROM_WIN32'. Generated bindings may be incomplete.", "Line 2, Column 9 in ClangUnsavedFile.h") };

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents, expectedDiagnostics: diagnostics);
        }
    }
}
