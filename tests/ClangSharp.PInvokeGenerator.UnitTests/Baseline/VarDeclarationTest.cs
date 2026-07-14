// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests.Baseline;

// Proof-of-concept: VarDeclarationTest migrated to the baseline model. One fixture, parameterized over
// all 16 variants; each case declares its C/C++ input and options exactly once. Compare against the
// legacy shape: an abstract Base/VarDeclarationTest.cs plus 16 near-identical folder copies.
[TestFixtureSource(nameof(Variants))]
public sealed class VarDeclarationTest : BaselineTest
{
    private static readonly string[] GuidMacroTestExcludedNames = ["GUID"];
    private static readonly string[] UncheckedConversionMacroTest2ExcludedNames = ["MyMacro1", "MyMacro2"];

    public VarDeclarationTest(BaselineVariant variant) : base(variant)
    {
    }

    protected override string Area => "VarDeclaration";

    [TestCase("double")]
    [TestCase("short")]
    [TestCase("int")]
    [TestCase("float")]
    public Task BasicTest(string nativeType)
        => ValidateAsync(nameof(BasicTest), $@"{nativeType} MyVariable = 0;", discriminator: nativeType);

    [TestCase("unsigned char")]
    [TestCase("long long")]
    [TestCase("signed char")]
    [TestCase("unsigned short")]
    [TestCase("unsigned int")]
    [TestCase("unsigned long long")]
    public Task BasicWithNativeTypeNameTest(string nativeType)
        => ValidateAsync(nameof(BasicWithNativeTypeNameTest), $@"{nativeType} MyVariable = 0;", discriminator: nativeType);

    [Test]
    public Task GuidMacroTest()
    {
        var inputContents = @"struct GUID {
    unsigned long  Data1;
    unsigned short Data2;
    unsigned short Data3;
    unsigned char  Data4[8];
};

const GUID IID_IUnknown = { 0x00000000, 0x0000, 0x0000, { 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46 } };
";

        var remappedNames = new Dictionary<string, string> { ["GUID"] = "Guid" };
        return ValidateAsync(nameof(GuidMacroTest), inputContents, excludedNames: GuidMacroTestExcludedNames, remappedNames: remappedNames);
    }

    [TestCase("0")]
    [TestCase("0U")]
    [TestCase("0LL")]
    [TestCase("0ULL")]
    [TestCase("0LLU")]
    [TestCase("0.0")]
    [TestCase("0.f")]
    public Task MacroTest(string nativeValue)
        => ValidateAsync(nameof(MacroTest), $@"#define MyMacro1 {nativeValue}
#define MyMacro2 MyMacro1", discriminator: nativeValue);

    [Test]
    public Task MultilineMacroTest()
        => ValidateAsync(nameof(MultilineMacroTest), "#define MyMacro1 0 + \\\n1");

    [TestCase("double")]
    [TestCase("short")]
    [TestCase("int")]
    [TestCase("float")]
    public Task NoInitializerTest(string nativeType)
        => ValidateAsync(nameof(NoInitializerTest), $@"{nativeType} MyVariable;", discriminator: nativeType);

    [Test]
    public Task Utf8StringLiteralMacroTest()
        => ValidateAsync(nameof(Utf8StringLiteralMacroTest), @"#define MyMacro1 ""Test\0\\\r\n\t\""""");

    [Test]
    public Task Utf16StringLiteralMacroTest()
        => ValidateAsync(nameof(Utf16StringLiteralMacroTest), @"#define MyMacro1 u""Test\0\\\r\n\t\""""");

    [Test]
    public Task WideStringLiteralConstTest()
        => ValidateAsync(nameof(WideStringLiteralConstTest), @"const wchar_t MyConst1[] = L""Test\0\\\r\n\t\"""";
const wchar_t* MyConst2 = L""Test\0\\\r\n\t\"""";
const wchar_t* const MyConst3 = L""Test\0\\\r\n\t\"""";");

    [Test]
    public Task StringLiteralConstTest()
        => ValidateAsync(nameof(StringLiteralConstTest), @"const char MyConst1[] = ""Test\0\\\r\n\t\"""";
const char* MyConst2 = ""Test\0\\\r\n\t\"""";
const char* const MyConst3 = ""Test\0\\\r\n\t\"""";");

    [Test]
    public Task WideStringLiteralStaticConstTest()
        => ValidateAsync(nameof(WideStringLiteralStaticConstTest), @"static const wchar_t MyConst1[] = L""Test\0\\\r\n\t\"""";
static const wchar_t* MyConst2 = L""Test\0\\\r\n\t\"""";
static const wchar_t* const MyConst3 = L""Test\0\\\r\n\t\"""";");

    [Test]
    public Task StringLiteralStaticConstTest()
        => ValidateAsync(nameof(StringLiteralStaticConstTest), @"static const char MyConst1[] = ""Test\0\\\r\n\t\"""";
static const char* MyConst2 = ""Test\0\\\r\n\t\"""";
static const char* const MyConst3 = ""Test\0\\\r\n\t\"""";");

    [Test]
    public Task UncheckedConversionMacroTest()
        => ValidateAsync(nameof(UncheckedConversionMacroTest), @"#define MyMacro1 (long)0x80000000L
#define MyMacro2 (int)0x80000000");

    [Test]
    public Task UncheckedFunctionLikeCastMacroTest()
        => ValidateAsync(nameof(UncheckedFunctionLikeCastMacroTest), @"#define MyMacro1 unsigned(-1)");

    [Test]
    public Task UncheckedConversionMacroTest2()
        => ValidateAsync(nameof(UncheckedConversionMacroTest2), @"#define MyMacro1(x, y, z) ((int)(((unsigned long)(x)<<31) | ((unsigned long)(y)<<16) | ((unsigned long)(z))))
#define MyMacro2(n) MyMacro1(1, 2, n)
#define MyMacro3 MyMacro2(3)", excludedNames: UncheckedConversionMacroTest2ExcludedNames);

    [Test]
    public Task UncheckedPointerMacroTest()
        => ValidateAsync(nameof(UncheckedPointerMacroTest), @"#define Macro1 ((int*) -1)");

    [Test]
    public Task UncheckedReinterpretCastMacroTest()
        => ValidateAsync(nameof(UncheckedReinterpretCastMacroTest), @"#define Macro1 reinterpret_cast<int*>(-1)");

    [Test]
    public Task MultidimensionlArrayTest()
        => ValidateAsync(nameof(MultidimensionlArrayTest), @"const int MyArray[2][2] = { { 0, 1 }, { 2, 3 } };");

    [Test]
    public Task ConditionalDefineConstTest()
    {
        var inputContents = @"typedef int TESTRESULT;
#define TESTRESULT_FROM_WIN32(x) ((TESTRESULT)(x) <= 0 ? ((TESTRESULT)(x)) : ((TESTRESULT) (((x) & 0x0000FFFF) | (7 << 16) | 0x80000000)))
#define ADDRESS_IN_USE TESTRESULT_FROM_WIN32(10048)";

        var diagnostics = new Diagnostic[] { new Diagnostic(DiagnosticLevel.Warning, "Function like macro definition records are not supported: 'TESTRESULT_FROM_WIN32'. Generated bindings may be incomplete.", "Line 2, Column 9 in ClangUnsavedFile.h") };
        return ValidateAsync(nameof(ConditionalDefineConstTest), inputContents, expectedDiagnostics: diagnostics);
    }

    [Test]
    public Task UndefinedFunctionLikeMacroTest()
    {
        var inputContents = @"#define ADDRESS_IN_USE TESTRESULT_FROM_WIN32(10048)";

        var diagnostics = new Diagnostic[] { new Diagnostic(DiagnosticLevel.Warning, "Macro definition 'ADDRESS_IN_USE' could not be resolved to a supported expression. Generated bindings may be incomplete.", "Line 2, Column 12 in ClangUnsavedFile.h") };
        return ValidateAsync(nameof(UndefinedFunctionLikeMacroTest), inputContents, expectedDiagnostics: diagnostics);
    }
}
