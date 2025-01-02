// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

public sealed class CTest()
    : PInvokeGeneratorTest(PInvokeGeneratorOutputMode.CSharp, PInvokeGeneratorConfigurationOptions.GenerateLatestCode)
{
    private const string DefaultCStandard = "c17";

    private static readonly string[] s_defaultCClangCommandLineArgs =
    [
        $"-std={DefaultCStandard}",                             // The input files should be compiled for C 17
        "-xc",                                  // The input files are C
    ];

    [Test]
    public Task BasicTest()
    {
        var inputContents = @"typedef enum MyEnum {
    MyEnum_Value0,
    MyEnum_Value1,
    MyEnum_Value2,
} enum_t;

typedef struct MyStruct {
    enum_t _field;
} struct_t;
";

        return ValidateGeneratedBindingsAsync(inputContents, commandLineArgs: s_defaultCClangCommandLineArgs, language: "c", languageStandard: DefaultCStandard);
    }

    [Test]
    public Task EnumTest()
    {
        var inputContents = @"enum {
    VALUE1 = 0,
    VALUE2,
    VALUE3
};

struct MyStruct {
    enum {
        VALUEA = 0,
        VALUEB,
        VALUEC
    } field;
};
";

        var diagnostics = new[] {
            new Diagnostic(DiagnosticLevel.Info, "Found anonymous enum: __AnonymousEnum_ClangUnsavedFile_L1_C1. Mapping values as constants in: Methods", "Line 1, Column 1 in ClangUnsavedFile.h"),
            new Diagnostic(DiagnosticLevel.Info, "Found anonymous enum: __AnonymousEnum_ClangUnsavedFile_L8_C5. Mapping values as constants in: Methods", "Line 8, Column 5 in ClangUnsavedFile.h")
        };
        return ValidateGeneratedBindingsAsync(inputContents, commandLineArgs: s_defaultCClangCommandLineArgs, expectedDiagnostics: diagnostics, language: "c", languageStandard: DefaultCStandard);
    }

    [Test]
    public Task StructTest()
    {
        var inputContents = @"typedef struct _MyStruct
{
    int _field;
} MyStruct;

typedef struct _MyOtherStruct
{
    MyStruct _field1;
    MyStruct* _field2;
} MyOtherStruct;

typedef struct _MyStructWithAnonymousStruct
{
    struct {
        int _field;
    } _anonymousStructField1;
} MyStructWithAnonymousStruct;

typedef struct _MyStructWithAnonymousUnion
{
    union {
        int _field1;
        int* _field2;
    } union1;
} MyStructWithAnonymousUnion;
";

        return ValidateGeneratedBindingsAsync(inputContents, commandLineArgs: s_defaultCClangCommandLineArgs, language: "c", languageStandard: DefaultCStandard);
    }

    [Test]
    public Task UnionTest()
    {
        var inputContents = @"typedef union _MyUnion
{
    int _field;
} MyUnion;

typedef union _MyOtherUnion
{
    MyUnion _field1;
    MyUnion* _field2;
} MyOtherUnion;
";

        return ValidateGeneratedBindingsAsync(inputContents, commandLineArgs: s_defaultCClangCommandLineArgs, language: "c", languageStandard: DefaultCStandard);
    }

    [Test]
    public Task MacroTest()
    {
        var inputContents = @"#define MyMacro1 5
#define MyMacro2 MyMacro1";

        return ValidateGeneratedBindingsAsync(inputContents, commandLineArgs: s_defaultCClangCommandLineArgs, language: "c", languageStandard: DefaultCStandard);
    }
}
