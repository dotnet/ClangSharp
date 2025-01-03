// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

[TestFixtureSource(nameof(FixtureArgs))]
public sealed class VarDeclarationTest(PInvokeGeneratorOutputMode outputMode, PInvokeGeneratorConfigurationOptions outputVersion)
    : PInvokeGeneratorTest(outputMode, outputVersion)
{
    private static readonly string[] s_guidMacroTestExcludedNames = ["GUID"];
    private static readonly string[] s_uncheckedConversionMacroTest2ExcludedNames = ["MyMacro1", "MyMacro2"];

    [TestCase("double")]
    [TestCase("short")]
    [TestCase("int")]
    [TestCase("float")]
    public Task BasicTest(string nativeType)
    {
        var inputContents = $@"{nativeType} MyVariable = 0;";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [TestCase("unsigned char")]
    [TestCase("long long")]
    [TestCase("signed char")]
    [TestCase("unsigned short")]
    [TestCase("unsigned int")]
    [TestCase("unsigned long long")]
    public Task BasicWithNativeTypeNameTest(string nativeType)
    {
        var inputContents = $@"{nativeType} MyVariable = 0;";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task GuidMacroTest()
    {
        var inputContents = $@"struct GUID {{
    unsigned long  Data1;
    unsigned short Data2;
    unsigned short Data3;
    unsigned char  Data4[8];
}};

const GUID IID_IUnknown = {{ 0x00000000, 0x0000, 0x0000, {{ 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46 }} }};
";

        var remappedNames = new Dictionary<string, string> { ["GUID"] = "Guid" };
        return ValidateGeneratedBindingsAsync(inputContents, excludedNames: s_guidMacroTestExcludedNames, remappedNames: remappedNames);
    }

    [TestCase("0")]
    [TestCase("0U")]
    [TestCase("0LL")]
    [TestCase("0ULL")]
    [TestCase("0LLU")]
    [TestCase("0.0")]
    [TestCase("0.f")]
    public Task MacroTest(string nativeValue)
    {
        var inputContents = $@"#define MyMacro1 {nativeValue}
#define MyMacro2 MyMacro1";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task MultilineMacroTest()
    {
        var inputContents = $@"#define MyMacro1 0 + \
1";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [TestCase("double")]
    [TestCase("short")]
    [TestCase("int")]
    [TestCase("float")]
    public Task NoInitializerTest(string nativeType)
    {
        var inputContents = $@"{nativeType} MyVariable;";
        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task Utf8StringLiteralMacroTest()
    {
        var inputContents = $@"#define MyMacro1 ""Test\0\\\r\n\t\""""";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task Utf16StringLiteralMacroTest()
    {
        var inputContents = $@"#define MyMacro1 u""Test\0\\\r\n\t\""""";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task WideStringLiteralConstTest()
    {
        var inputContents = $@"const wchar_t MyConst1[] = L""Test\0\\\r\n\t\"""";
const wchar_t* MyConst2 = L""Test\0\\\r\n\t\"""";
const wchar_t* const MyConst3 = L""Test\0\\\r\n\t\"""";";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task StringLiteralConstTest()
    {
        var inputContents = $@"const char MyConst1[] = ""Test\0\\\r\n\t\"""";
const char* MyConst2 = ""Test\0\\\r\n\t\"""";
const char* const MyConst3 = ""Test\0\\\r\n\t\"""";";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task WideStringLiteralStaticConstTest()
    {
        var inputContents = $@"static const wchar_t MyConst1[] = L""Test\0\\\r\n\t\"""";
static const wchar_t* MyConst2 = L""Test\0\\\r\n\t\"""";
static const wchar_t* const MyConst3 = L""Test\0\\\r\n\t\"""";";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task StringLiteralStaticConstTest()
    {
        var inputContents = $@"static const char MyConst1[] = ""Test\0\\\r\n\t\"""";
static const char* MyConst2 = ""Test\0\\\r\n\t\"""";
static const char* const MyConst3 = ""Test\0\\\r\n\t\"""";";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task UncheckedConversionMacroTest()
    {
        var inputContents = $@"#define MyMacro1 (long)0x80000000L
#define MyMacro2 (int)0x80000000";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task UncheckedFunctionLikeCastMacroTest()
    {
        var inputContents = $@"#define MyMacro1 unsigned(-1)";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task UncheckedConversionMacroTest2()
    {
        var inputContents = $@"#define MyMacro1(x, y, z) ((int)(((unsigned long)(x)<<31) | ((unsigned long)(y)<<16) | ((unsigned long)(z))))
#define MyMacro2(n) MyMacro1(1, 2, n)
#define MyMacro3 MyMacro2(3)";

        return ValidateGeneratedBindingsAsync(inputContents, excludedNames: s_uncheckedConversionMacroTest2ExcludedNames);
    }

    [Test]
    public Task UncheckedPointerMacroTest()
    {
        var inputContents = $@"#define Macro1 ((int*) -1)";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task UncheckedReinterpretCastMacroTest()
    {
        var inputContents = $@"#define Macro1 reinterpret_cast<int*>(-1)";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task MultidimensionlArrayTest()
    {
        var inputContents = $@"const int MyArray[2][2] = {{ {{ 0, 1 }}, {{ 2, 3 }} }};";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task ConditionalDefineConstTest()
    {
        var inputContents = @"typedef int TESTRESULT;
#define TESTRESULT_FROM_WIN32(x) ((TESTRESULT)(x) <= 0 ? ((TESTRESULT)(x)) : ((TESTRESULT) (((x) & 0x0000FFFF) | (7 << 16) | 0x80000000)))
#define ADDRESS_IN_USE TESTRESULT_FROM_WIN32(10048)";
        var diagnostics = new Diagnostic[] { new Diagnostic(DiagnosticLevel.Warning, "Function like macro definition records are not supported: 'TESTRESULT_FROM_WIN32'. Generated bindings may be incomplete.", "Line 2, Column 9 in ClangUnsavedFile.h") };

        return ValidateGeneratedBindingsAsync(inputContents, expectedDiagnostics: diagnostics);
    }
}
