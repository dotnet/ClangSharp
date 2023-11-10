// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

[Platform("unix")]
public sealed class CSharpLatestUnix_VarDeclarationTest : VarDeclarationTest
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

        return ValidateGeneratedCSharpLatestUnixBindingsAsync(inputContents, expectedOutputContents);
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

        return ValidateGeneratedCSharpLatestUnixBindingsAsync(inputContents, expectedOutputContents);
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

        var remappedNames = new Dictionary<string, string> { ["GUID"] = "Guid" };
        return ValidateGeneratedCSharpLatestUnixBindingsAsync(inputContents, expectedOutputContents, excludedNames: GuidMacroTestExcludedNames, remappedNames: remappedNames);
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

        return ValidateGeneratedCSharpLatestUnixBindingsAsync(inputContents, expectedOutputContents);
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

        return ValidateGeneratedCSharpLatestUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task NoInitializerTestImpl(string nativeType)
    {
        var inputContents = $@"{nativeType} MyVariable;";
        var expectedOutputContents = "";
        return ValidateGeneratedCSharpLatestUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task Utf8StringLiteralMacroTestImpl()
    {
        var inputContents = $@"#define MyMacro1 ""Test\0\\\r\n\t\""""";

        var expectedOutputContents = $@"using System;

namespace ClangSharp.Test
{{
    public static partial class Methods
    {{
        [NativeTypeName(""#define MyMacro1 \""Test\0\\\r\n\t\""\"""")]
        public static ReadOnlySpan<byte> MyMacro1 => ""Test\0\\\r\n\t\""""u8;
    }}
}}
";

        return ValidateGeneratedCSharpLatestUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task Utf16StringLiteralMacroTestImpl()
    {
        var inputContents = $@"#define MyMacro1 u""Test\0\\\r\n\t\""""";

        var expectedOutputContents = $@"namespace ClangSharp.Test
{{
    public static partial class Methods
    {{
        [NativeTypeName(""#define MyMacro1 u\""Test\0\\\r\n\t\""\"""")]
        public const string MyMacro1 = ""Test\0\\\r\n\t\"""";
    }}
}}
";

        return ValidateGeneratedCSharpLatestUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task WideStringLiteralConstTestImpl()
    {
        var inputContents = $@"const wchar_t MyConst1[] = L""Test\0\\\r\n\t\"""";
const wchar_t* MyConst2 = L""Test\0\\\r\n\t\"""";
const wchar_t* const MyConst3 = L""Test\0\\\r\n\t\"""";";

        var expectedOutputContents = $@"namespace ClangSharp.Test
{{
    public static partial class Methods
    {{
        [NativeTypeName(""const wchar_t[11]"")]
        public static readonly uint[] MyConst1 = new uint[] {{ 0x00000054, 0x00000065, 0x00000073, 0x00000074, 0x00000000, 0x0000005C, 0x0000000D, 0x0000000A, 0x00000009, 0x00000022, 0x00000000 }};

        [NativeTypeName(""const wchar_t *"")]
        public static uint[] MyConst2 = new uint[] {{ 0x00000054, 0x00000065, 0x00000073, 0x00000074, 0x00000000, 0x0000005C, 0x0000000D, 0x0000000A, 0x00000009, 0x00000022, 0x00000000 }};

        [NativeTypeName(""const wchar_t *const"")]
        public static readonly uint[] MyConst3 = new uint[] {{ 0x00000054, 0x00000065, 0x00000073, 0x00000074, 0x00000000, 0x0000005C, 0x0000000D, 0x0000000A, 0x00000009, 0x00000022, 0x00000000 }};
    }}
}}
";

        return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task StringLiteralConstTestImpl()
    {
        var inputContents = $@"const char MyConst1[] = ""Test\0\\\r\n\t\"""";
const char* MyConst2 = ""Test\0\\\r\n\t\"""";
const char* const MyConst3 = ""Test\0\\\r\n\t\"""";";

        var expectedOutputContents = $@"using System;

namespace ClangSharp.Test
{{
    public static partial class Methods
    {{
        [NativeTypeName(""const char[11]"")]
        public static ReadOnlySpan<byte> MyConst1 => ""Test\0\\\r\n\t\""""u8;

        [NativeTypeName(""const char *"")]
        public static byte[] MyConst2 = ""Test\0\\\r\n\t\""""u8.ToArray();

        [NativeTypeName(""const char *const"")]
        public static ReadOnlySpan<byte> MyConst3 => ""Test\0\\\r\n\t\""""u8;
    }}
}}
";

        return ValidateGeneratedCSharpLatestUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task WideStringLiteralStaticConstTestImpl()
    {
        var inputContents = $@"static const wchar_t MyConst1[] = L""Test\0\\\r\n\t\"""";
static const wchar_t* MyConst2 = L""Test\0\\\r\n\t\"""";
static const wchar_t* const MyConst3 = L""Test\0\\\r\n\t\"""";";

        var expectedOutputContents = $@"namespace ClangSharp.Test
{{
    public static partial class Methods
    {{
        [NativeTypeName(""const wchar_t[11]"")]
        public static readonly uint[] MyConst1 = new uint[] {{ 0x00000054, 0x00000065, 0x00000073, 0x00000074, 0x00000000, 0x0000005C, 0x0000000D, 0x0000000A, 0x00000009, 0x00000022, 0x00000000 }};

        [NativeTypeName(""const wchar_t *"")]
        public static uint[] MyConst2 = new uint[] {{ 0x00000054, 0x00000065, 0x00000073, 0x00000074, 0x00000000, 0x0000005C, 0x0000000D, 0x0000000A, 0x00000009, 0x00000022, 0x00000000 }};

        [NativeTypeName(""const wchar_t *const"")]
        public static readonly uint[] MyConst3 = new uint[] {{ 0x00000054, 0x00000065, 0x00000073, 0x00000074, 0x00000000, 0x0000005C, 0x0000000D, 0x0000000A, 0x00000009, 0x00000022, 0x00000000 }};
    }}
}}
";

        return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task StringLiteralStaticConstTestImpl()
    {
        var inputContents = $@"static const char MyConst1[] = ""Test\0\\\r\n\t\"""";
static const char* MyConst2 = ""Test\0\\\r\n\t\"""";
static const char* const MyConst3 = ""Test\0\\\r\n\t\"""";";

        var expectedOutputContents = $@"using System;

namespace ClangSharp.Test
{{
    public static partial class Methods
    {{
        [NativeTypeName(""const char[11]"")]
        public static ReadOnlySpan<byte> MyConst1 => ""Test\0\\\r\n\t\""""u8;

        [NativeTypeName(""const char *"")]
        public static byte[] MyConst2 = ""Test\0\\\r\n\t\""""u8.ToArray();

        [NativeTypeName(""const char *const"")]
        public static ReadOnlySpan<byte> MyConst3 => ""Test\0\\\r\n\t\""""u8;
    }}
}}
";

        return ValidateGeneratedCSharpLatestUnixBindingsAsync(inputContents, expectedOutputContents);
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
        public const nint MyMacro1 = unchecked((nint)(0x80000000));

        [NativeTypeName(""#define MyMacro2 (int)0x80000000"")]
        public const int MyMacro2 = unchecked((int)(0x80000000));
    }}
}}
";

        return ValidateGeneratedCSharpLatestUnixBindingsAsync(inputContents, expectedOutputContents);
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

        return ValidateGeneratedCSharpLatestUnixBindingsAsync(inputContents, expectedOutputContents);
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
        public const int MyMacro3 = unchecked((int)(((nuint)(1) << 31) | ((nuint)(2) << 16) | ((nuint)(3))));
    }}
}}
";

        return ValidateGeneratedCSharpLatestUnixBindingsAsync(inputContents, expectedOutputContents, excludedNames: UncheckedConversionMacroTest2ExcludedNames);
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

        return ValidateGeneratedCSharpLatestUnixBindingsAsync(inputContents, expectedOutputContents);
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

        return ValidateGeneratedCSharpLatestUnixBindingsAsync(inputContents, expectedOutputContents);
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

        return ValidateGeneratedCSharpLatestUnixBindingsAsync(inputContents, expectedOutputContents);
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

        return ValidateGeneratedCSharpLatestUnixBindingsAsync(inputContents, expectedOutputContents, expectedDiagnostics: diagnostics);
    }
}
