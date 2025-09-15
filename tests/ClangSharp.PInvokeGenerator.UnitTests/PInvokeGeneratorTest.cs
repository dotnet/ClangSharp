// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Xml.Linq;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXTranslationUnit_Flags;
using System.Xml.Serialization;
using ClangSharp.XML;
using System.Xml;
using System.Text;
using System;
using System.Text.RegularExpressions;

namespace ClangSharp.UnitTests;

public abstract partial class PInvokeGeneratorTest
{
    protected const string DefaultInputFileName = "ClangUnsavedFile.h";
    protected const string DefaultLibraryPath = "ClangSharpPInvokeGenerator";
    protected const string DefaultNamespaceName = "ClangSharp.Test";

    protected const CXTranslationUnit_Flags DefaultTranslationUnitFlags = CXTranslationUnit_IncludeAttributedTypes          // Include attributed types in CXType
                                                                        | CXTranslationUnit_VisitImplicitAttributes         // Implicit attributes should be visited
                                                                        | CXTranslationUnit_DetailedPreprocessingRecord;
    protected const string DefaultCStandard = "c17";
    protected const string DefaultCppStandard = "c++17";

    protected static readonly string[] DefaultCClangCommandLineArgs =
    [
        $"-std={DefaultCStandard}",                             // The input files should be compiled for C 17
        "-xc",                                  // The input files are C
    ];

    protected static readonly string[] DefaultCppClangCommandLineArgs =
    [
        $"-std={DefaultCppStandard}",                           // The input files should be compiled for C++ 17
        "-xc++",                                // The input files are C++
        "-Wno-pragma-once-outside-header",      // We are processing files which may be header files
        "-Wno-c++11-narrowing"
    ];

    protected static string EscapeXml(string value) => new XText(value).ToString();

    protected static Task ValidateGeneratedCSharpPreviewWindowsBindingsAsync(string inputContents, string expectedOutputContents, PInvokeGeneratorConfigurationOptions additionalConfigOptions = PInvokeGeneratorConfigurationOptions.None, string[]? excludedNames = null, IReadOnlyDictionary<string, string>? remappedNames = null, IReadOnlyDictionary<string, AccessSpecifier>? withAccessSpecifiers = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withAttributes = null, IReadOnlyDictionary<string, string>? withCallConvs = null, IReadOnlyDictionary<string, string>? withClasses = null, IReadOnlyDictionary<string, string>? withLibraryPaths = null, IReadOnlyDictionary<string, string>? withNamespaces = null, string[]? withSetLastErrors = null, IReadOnlyDictionary<string, (string, PInvokeGeneratorTransparentStructKind)>? withTransparentStructs = null, IReadOnlyDictionary<string, string>? withTypes = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withUsings = null, IReadOnlyDictionary<string, string>? withPackings = null, IEnumerable<Diagnostic>? expectedDiagnostics = null, string libraryPath = DefaultLibraryPath, string[]? commandLineArgs = null, string language = "c++", string languageStandard = DefaultCppStandard)
        => ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorOutputMode.CSharp, PInvokeGeneratorConfigurationOptions.GeneratePreviewCode | PInvokeGeneratorConfigurationOptions.None | additionalConfigOptions, excludedNames, remappedNames, withAccessSpecifiers, withAttributes, withCallConvs, withClasses, withLibraryPaths, withNamespaces, withSetLastErrors, withTransparentStructs, withTypes, withUsings, withPackings, expectedDiagnostics, libraryPath, commandLineArgs, language, languageStandard);

    protected static Task ValidateGeneratedCSharpPreviewUnixBindingsAsync(string inputContents, string expectedOutputContents, PInvokeGeneratorConfigurationOptions additionalConfigOptions = PInvokeGeneratorConfigurationOptions.None, string[]? excludedNames = null, IReadOnlyDictionary<string, string>? remappedNames = null, IReadOnlyDictionary<string, AccessSpecifier>? withAccessSpecifiers = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withAttributes = null, IReadOnlyDictionary<string, string>? withCallConvs = null, IReadOnlyDictionary<string, string>? withClasses = null, IReadOnlyDictionary<string, string>? withLibraryPaths = null, IReadOnlyDictionary<string, string>? withNamespaces = null, string[]? withSetLastErrors = null, IReadOnlyDictionary<string, (string, PInvokeGeneratorTransparentStructKind)>? withTransparentStructs = null, IReadOnlyDictionary<string, string>? withTypes = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withUsings = null, IReadOnlyDictionary<string, string>? withPackings = null, IEnumerable<Diagnostic>? expectedDiagnostics = null, string libraryPath = DefaultLibraryPath, string[]? commandLineArgs = null, string language = "c++", string languageStandard = DefaultCppStandard)
        => ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorOutputMode.CSharp, PInvokeGeneratorConfigurationOptions.GeneratePreviewCode | PInvokeGeneratorConfigurationOptions.GenerateUnixTypes | additionalConfigOptions, excludedNames, remappedNames, withAccessSpecifiers, withAttributes, withCallConvs, withClasses, withLibraryPaths, withNamespaces, withSetLastErrors, withTransparentStructs, withTypes, withUsings, withPackings, expectedDiagnostics, libraryPath, commandLineArgs, language, languageStandard);

    protected static Task ValidateGeneratedCSharpLatestWindowsBindingsAsync(string inputContents, string expectedOutputContents, PInvokeGeneratorConfigurationOptions additionalConfigOptions = PInvokeGeneratorConfigurationOptions.None, string[]? excludedNames = null, IReadOnlyDictionary<string, string>? remappedNames = null, IReadOnlyDictionary<string, AccessSpecifier>? withAccessSpecifiers = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withAttributes = null, IReadOnlyDictionary<string, string>? withCallConvs = null, IReadOnlyDictionary<string, string>? withClasses = null, IReadOnlyDictionary<string, string>? withLibraryPaths = null, IReadOnlyDictionary<string, string>? withNamespaces = null, string[]? withSetLastErrors = null, IReadOnlyDictionary<string, (string, PInvokeGeneratorTransparentStructKind)>? withTransparentStructs = null, IReadOnlyDictionary<string, string>? withTypes = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withUsings = null, IReadOnlyDictionary<string, string>? withPackings = null, IEnumerable<Diagnostic>? expectedDiagnostics = null, string libraryPath = DefaultLibraryPath, string[]? commandLineArgs = null, string language = "c++", string languageStandard = DefaultCppStandard)
        => ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorOutputMode.CSharp, PInvokeGeneratorConfigurationOptions.GenerateLatestCode | PInvokeGeneratorConfigurationOptions.None | additionalConfigOptions, excludedNames, remappedNames, withAccessSpecifiers, withAttributes, withCallConvs, withClasses, withLibraryPaths, withNamespaces, withSetLastErrors, withTransparentStructs, withTypes, withUsings, withPackings, expectedDiagnostics, libraryPath, commandLineArgs, language, languageStandard);

    protected static Task ValidateGeneratedCSharpLatestUnixBindingsAsync(string inputContents, string expectedOutputContents, PInvokeGeneratorConfigurationOptions additionalConfigOptions = PInvokeGeneratorConfigurationOptions.None, string[]? excludedNames = null, IReadOnlyDictionary<string, string>? remappedNames = null, IReadOnlyDictionary<string, AccessSpecifier>? withAccessSpecifiers = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withAttributes = null, IReadOnlyDictionary<string, string>? withCallConvs = null, IReadOnlyDictionary<string, string>? withClasses = null, IReadOnlyDictionary<string, string>? withLibraryPaths = null, IReadOnlyDictionary<string, string>? withNamespaces = null, string[]? withSetLastErrors = null, IReadOnlyDictionary<string, (string, PInvokeGeneratorTransparentStructKind)>? withTransparentStructs = null, IReadOnlyDictionary<string, string>? withTypes = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withUsings = null, IReadOnlyDictionary<string, string>? withPackings = null, IEnumerable<Diagnostic>? expectedDiagnostics = null, string libraryPath = DefaultLibraryPath, string[]? commandLineArgs = null, string language = "c++", string languageStandard = DefaultCppStandard)
        => ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorOutputMode.CSharp, PInvokeGeneratorConfigurationOptions.GenerateLatestCode | PInvokeGeneratorConfigurationOptions.GenerateUnixTypes | additionalConfigOptions, excludedNames, remappedNames, withAccessSpecifiers, withAttributes, withCallConvs, withClasses, withLibraryPaths, withNamespaces, withSetLastErrors, withTransparentStructs, withTypes, withUsings, withPackings, expectedDiagnostics, libraryPath, commandLineArgs, language, languageStandard);

    protected static Task ValidateGeneratedCSharpDefaultWindowsBindingsAsync(string inputContents, string expectedOutputContents, PInvokeGeneratorConfigurationOptions additionalConfigOptions = PInvokeGeneratorConfigurationOptions.None, string[]? excludedNames = null, IReadOnlyDictionary<string, string>? remappedNames = null, IReadOnlyDictionary<string, AccessSpecifier>? withAccessSpecifiers = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withAttributes = null, IReadOnlyDictionary<string, string>? withCallConvs = null, IReadOnlyDictionary<string, string>? withClasses = null, IReadOnlyDictionary<string, string>? withLibraryPaths = null, IReadOnlyDictionary<string, string>? withNamespaces = null, string[]? withSetLastErrors = null, IReadOnlyDictionary<string, (string, PInvokeGeneratorTransparentStructKind)>? withTransparentStructs = null, IReadOnlyDictionary<string, string>? withTypes = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withUsings = null, IReadOnlyDictionary<string, string>? withPackings = null, IEnumerable<Diagnostic>? expectedDiagnostics = null, string libraryPath = DefaultLibraryPath, string[]? commandLineArgs = null, string language = "c++", string languageStandard = DefaultCppStandard)
        => ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorOutputMode.CSharp, PInvokeGeneratorConfigurationOptions.None | additionalConfigOptions, excludedNames, remappedNames, withAccessSpecifiers, withAttributes, withCallConvs, withClasses, withLibraryPaths, withNamespaces, withSetLastErrors, withTransparentStructs, withTypes, withUsings, withPackings, expectedDiagnostics, libraryPath, commandLineArgs, language, languageStandard);

    protected static Task ValidateGeneratedCSharpDefaultUnixBindingsAsync(string inputContents, string expectedOutputContents, PInvokeGeneratorConfigurationOptions additionalConfigOptions = PInvokeGeneratorConfigurationOptions.None, string[]? excludedNames = null, IReadOnlyDictionary<string, string>? remappedNames = null, IReadOnlyDictionary<string, AccessSpecifier>? withAccessSpecifiers = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withAttributes = null, IReadOnlyDictionary<string, string>? withCallConvs = null, IReadOnlyDictionary<string, string>? withClasses = null, IReadOnlyDictionary<string, string>? withLibraryPaths = null, IReadOnlyDictionary<string, string>? withNamespaces = null, string[]? withSetLastErrors = null, IReadOnlyDictionary<string, (string, PInvokeGeneratorTransparentStructKind)>? withTransparentStructs = null, IReadOnlyDictionary<string, string>? withTypes = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withUsings = null, IReadOnlyDictionary<string, string>? withPackings = null, IEnumerable<Diagnostic>? expectedDiagnostics = null, string libraryPath = DefaultLibraryPath, string[]? commandLineArgs = null, string language = "c++", string languageStandard = DefaultCppStandard)
        => ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorOutputMode.CSharp, PInvokeGeneratorConfigurationOptions.GenerateUnixTypes | additionalConfigOptions, excludedNames, remappedNames, withAccessSpecifiers, withAttributes, withCallConvs, withClasses, withLibraryPaths, withNamespaces, withSetLastErrors, withTransparentStructs, withTypes, withUsings, withPackings, expectedDiagnostics, libraryPath, commandLineArgs, language, languageStandard);

    protected static Task ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(string inputContents, string expectedOutputContents, PInvokeGeneratorConfigurationOptions additionalConfigOptions = PInvokeGeneratorConfigurationOptions.None, string[]? excludedNames = null, IReadOnlyDictionary<string, string>? remappedNames = null, IReadOnlyDictionary<string, AccessSpecifier>? withAccessSpecifiers = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withAttributes = null, IReadOnlyDictionary<string, string>? withCallConvs = null, IReadOnlyDictionary<string, string>? withClasses = null, IReadOnlyDictionary<string, string>? withLibraryPaths = null, IReadOnlyDictionary<string, string>? withNamespaces = null, string[]? withSetLastErrors = null, IReadOnlyDictionary<string, (string, PInvokeGeneratorTransparentStructKind)>? withTransparentStructs = null, IReadOnlyDictionary<string, string>? withTypes = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withUsings = null, IReadOnlyDictionary<string, string>? withPackings = null, IEnumerable<Diagnostic>? expectedDiagnostics = null, string libraryPath = DefaultLibraryPath, string[]? commandLineArgs = null, string language = "c++", string languageStandard = DefaultCppStandard)
        => ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorOutputMode.CSharp, PInvokeGeneratorConfigurationOptions.GenerateCompatibleCode | additionalConfigOptions, excludedNames, remappedNames, withAccessSpecifiers, withAttributes, withCallConvs, withClasses, withLibraryPaths, withNamespaces, withSetLastErrors, withTransparentStructs, withTypes, withUsings, withPackings, expectedDiagnostics, libraryPath, commandLineArgs, language, languageStandard);

    protected static Task ValidateGeneratedCSharpCompatibleUnixBindingsAsync(string inputContents, string expectedOutputContents, PInvokeGeneratorConfigurationOptions additionalConfigOptions = PInvokeGeneratorConfigurationOptions.None, string[]? excludedNames = null, IReadOnlyDictionary<string, string>? remappedNames = null, IReadOnlyDictionary<string, AccessSpecifier>? withAccessSpecifiers = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withAttributes = null, IReadOnlyDictionary<string, string>? withCallConvs = null, IReadOnlyDictionary<string, string>? withClasses = null, IReadOnlyDictionary<string, string>? withLibraryPaths = null, IReadOnlyDictionary<string, string>? withNamespaces = null, string[]? withSetLastErrors = null, IReadOnlyDictionary<string, (string, PInvokeGeneratorTransparentStructKind)>? withTransparentStructs = null, IReadOnlyDictionary<string, string>? withTypes = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withUsings = null, IReadOnlyDictionary<string, string>? withPackings = null, IEnumerable<Diagnostic>? expectedDiagnostics = null, string libraryPath = DefaultLibraryPath, string[]? commandLineArgs = null, string language = "c++", string languageStandard = DefaultCppStandard)
        => ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorOutputMode.CSharp, PInvokeGeneratorConfigurationOptions.GenerateCompatibleCode | PInvokeGeneratorConfigurationOptions.GenerateUnixTypes | additionalConfigOptions, excludedNames, remappedNames, withAccessSpecifiers, withAttributes, withCallConvs, withClasses, withLibraryPaths, withNamespaces, withSetLastErrors, withTransparentStructs, withTypes, withUsings, withPackings, expectedDiagnostics, libraryPath, commandLineArgs, language, languageStandard);

    protected static Task ValidateGeneratedXmlPreviewWindowsBindingsAsync(string inputContents, string expectedOutputContents, PInvokeGeneratorConfigurationOptions additionalConfigOptions = PInvokeGeneratorConfigurationOptions.None, string[]? excludedNames = null, IReadOnlyDictionary<string, string>? remappedNames = null, IReadOnlyDictionary<string, AccessSpecifier>? withAccessSpecifiers = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withAttributes = null, IReadOnlyDictionary<string, string>? withCallConvs = null, IReadOnlyDictionary<string, string>? withClasses = null, IReadOnlyDictionary<string, string>? withLibraryPaths = null, IReadOnlyDictionary<string, string>? withNamespaces = null, string[]? withSetLastErrors = null, IReadOnlyDictionary<string, (string, PInvokeGeneratorTransparentStructKind)>? withTransparentStructs = null, IReadOnlyDictionary<string, string>? withTypes = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withUsings = null, IReadOnlyDictionary<string, string>? withPackings = null, IEnumerable<Diagnostic>? expectedDiagnostics = null, string libraryPath = DefaultLibraryPath, string[]? commandLineArgs = null, string language = "c++", string languageStandard = DefaultCppStandard)
        => ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorOutputMode.Xml, PInvokeGeneratorConfigurationOptions.GeneratePreviewCode | additionalConfigOptions, excludedNames, remappedNames, withAccessSpecifiers, withAttributes, withCallConvs, withClasses, withLibraryPaths, withNamespaces, withSetLastErrors, withTransparentStructs, withTypes, withUsings, withPackings, expectedDiagnostics, libraryPath, commandLineArgs, language, languageStandard);

    protected static Task ValidateGeneratedXmlPreviewUnixBindingsAsync(string inputContents, string expectedOutputContents, PInvokeGeneratorConfigurationOptions additionalConfigOptions = PInvokeGeneratorConfigurationOptions.None, string[]? excludedNames = null, IReadOnlyDictionary<string, string>? remappedNames = null, IReadOnlyDictionary<string, AccessSpecifier>? withAccessSpecifiers = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withAttributes = null, IReadOnlyDictionary<string, string>? withCallConvs = null, IReadOnlyDictionary<string, string>? withClasses = null, IReadOnlyDictionary<string, string>? withLibraryPaths = null, IReadOnlyDictionary<string, string>? withNamespaces = null, string[]? withSetLastErrors = null, IReadOnlyDictionary<string, (string, PInvokeGeneratorTransparentStructKind)>? withTransparentStructs = null, IReadOnlyDictionary<string, string>? withTypes = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withUsings = null, IReadOnlyDictionary<string, string>? withPackings = null, IEnumerable<Diagnostic>? expectedDiagnostics = null, string libraryPath = DefaultLibraryPath, string[]? commandLineArgs = null, string language = "c++", string languageStandard = DefaultCppStandard)
        => ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorOutputMode.Xml, PInvokeGeneratorConfigurationOptions.GeneratePreviewCode | PInvokeGeneratorConfigurationOptions.GenerateUnixTypes | additionalConfigOptions, excludedNames, remappedNames, withAccessSpecifiers, withAttributes, withCallConvs, withClasses, withLibraryPaths, withNamespaces, withSetLastErrors, withTransparentStructs, withTypes, withUsings, withPackings, expectedDiagnostics, libraryPath, commandLineArgs, language, languageStandard);

    protected static Task ValidateGeneratedXmlLatestWindowsBindingsAsync(string inputContents, string expectedOutputContents, PInvokeGeneratorConfigurationOptions additionalConfigOptions = PInvokeGeneratorConfigurationOptions.None, string[]? excludedNames = null, IReadOnlyDictionary<string, string>? remappedNames = null, IReadOnlyDictionary<string, AccessSpecifier>? withAccessSpecifiers = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withAttributes = null, IReadOnlyDictionary<string, string>? withCallConvs = null, IReadOnlyDictionary<string, string>? withClasses = null, IReadOnlyDictionary<string, string>? withLibraryPaths = null, IReadOnlyDictionary<string, string>? withNamespaces = null, string[]? withSetLastErrors = null, IReadOnlyDictionary<string, (string, PInvokeGeneratorTransparentStructKind)>? withTransparentStructs = null, IReadOnlyDictionary<string, string>? withTypes = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withUsings = null, IReadOnlyDictionary<string, string>? withPackings = null, IEnumerable<Diagnostic>? expectedDiagnostics = null, string libraryPath = DefaultLibraryPath, string[]? commandLineArgs = null, string language = "c++", string languageStandard = DefaultCppStandard)
        => ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorOutputMode.Xml, PInvokeGeneratorConfigurationOptions.GenerateLatestCode | PInvokeGeneratorConfigurationOptions.None | additionalConfigOptions, excludedNames, remappedNames, withAccessSpecifiers, withAttributes, withCallConvs, withClasses, withLibraryPaths, withNamespaces, withSetLastErrors, withTransparentStructs, withTypes, withUsings, withPackings, expectedDiagnostics, libraryPath, commandLineArgs, language, languageStandard);

    protected static Task ValidateGeneratedXmlLatestUnixBindingsAsync(string inputContents, string expectedOutputContents, PInvokeGeneratorConfigurationOptions additionalConfigOptions = PInvokeGeneratorConfigurationOptions.None, string[]? excludedNames = null, IReadOnlyDictionary<string, string>? remappedNames = null, IReadOnlyDictionary<string, AccessSpecifier>? withAccessSpecifiers = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withAttributes = null, IReadOnlyDictionary<string, string>? withCallConvs = null, IReadOnlyDictionary<string, string>? withClasses = null, IReadOnlyDictionary<string, string>? withLibraryPaths = null, IReadOnlyDictionary<string, string>? withNamespaces = null, string[]? withSetLastErrors = null, IReadOnlyDictionary<string, (string, PInvokeGeneratorTransparentStructKind)>? withTransparentStructs = null, IReadOnlyDictionary<string, string>? withTypes = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withUsings = null, IReadOnlyDictionary<string, string>? withPackings = null, IEnumerable<Diagnostic>? expectedDiagnostics = null, string libraryPath = DefaultLibraryPath, string[]? commandLineArgs = null, string language = "c++", string languageStandard = DefaultCppStandard)
        => ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorOutputMode.Xml, PInvokeGeneratorConfigurationOptions.GenerateLatestCode | PInvokeGeneratorConfigurationOptions.GenerateUnixTypes | additionalConfigOptions, excludedNames, remappedNames, withAccessSpecifiers, withAttributes, withCallConvs, withClasses, withLibraryPaths, withNamespaces, withSetLastErrors, withTransparentStructs, withTypes, withUsings, withPackings, expectedDiagnostics, libraryPath, commandLineArgs, language, languageStandard);

    protected static Task ValidateGeneratedXmlDefaultWindowsBindingsAsync(string inputContents, string expectedOutputContents, PInvokeGeneratorConfigurationOptions additionalConfigOptions = PInvokeGeneratorConfigurationOptions.None, string[]? excludedNames = null, IReadOnlyDictionary<string, string>? remappedNames = null, IReadOnlyDictionary<string, AccessSpecifier>? withAccessSpecifiers = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withAttributes = null, IReadOnlyDictionary<string, string>? withCallConvs = null, IReadOnlyDictionary<string, string>? withClasses = null, IReadOnlyDictionary<string, string>? withLibraryPaths = null, IReadOnlyDictionary<string, string>? withNamespaces = null, string[]? withSetLastErrors = null, IReadOnlyDictionary<string, (string, PInvokeGeneratorTransparentStructKind)>? withTransparentStructs = null, IReadOnlyDictionary<string, string>? withTypes = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withUsings = null, IReadOnlyDictionary<string, string>? withPackings = null, IEnumerable<Diagnostic>? expectedDiagnostics = null, string libraryPath = DefaultLibraryPath, string[]? commandLineArgs = null, string language = "c++", string languageStandard = DefaultCppStandard)
        => ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorOutputMode.Xml, PInvokeGeneratorConfigurationOptions.None | additionalConfigOptions, excludedNames, remappedNames, withAccessSpecifiers, withAttributes, withCallConvs, withClasses, withLibraryPaths, withNamespaces, withSetLastErrors, withTransparentStructs, withTypes, withUsings, withPackings, expectedDiagnostics, libraryPath, commandLineArgs, language, languageStandard);

    protected static Task ValidateGeneratedXmlDefaultUnixBindingsAsync(string inputContents, string expectedOutputContents, PInvokeGeneratorConfigurationOptions additionalConfigOptions = PInvokeGeneratorConfigurationOptions.None, string[]? excludedNames = null, IReadOnlyDictionary<string, string>? remappedNames = null, IReadOnlyDictionary<string, AccessSpecifier>? withAccessSpecifiers = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withAttributes = null, IReadOnlyDictionary<string, string>? withCallConvs = null, IReadOnlyDictionary<string, string>? withClasses = null, IReadOnlyDictionary<string, string>? withLibraryPaths = null, IReadOnlyDictionary<string, string>? withNamespaces = null, string[]? withSetLastErrors = null, IReadOnlyDictionary<string, (string, PInvokeGeneratorTransparentStructKind)>? withTransparentStructs = null, IReadOnlyDictionary<string, string>? withTypes = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withUsings = null, IReadOnlyDictionary<string, string>? withPackings = null, IEnumerable<Diagnostic>? expectedDiagnostics = null, string libraryPath = DefaultLibraryPath, string[]? commandLineArgs = null, string language = "c++", string languageStandard = DefaultCppStandard)
        => ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorOutputMode.Xml, PInvokeGeneratorConfigurationOptions.GenerateUnixTypes | additionalConfigOptions, excludedNames, remappedNames, withAccessSpecifiers, withAttributes, withCallConvs, withClasses, withLibraryPaths, withNamespaces, withSetLastErrors, withTransparentStructs, withTypes, withUsings, withPackings, expectedDiagnostics, libraryPath, commandLineArgs, language, languageStandard);

    protected static Task ValidateGeneratedXmlCompatibleWindowsBindingsAsync(string inputContents, string expectedOutputContents, PInvokeGeneratorConfigurationOptions additionalConfigOptions = PInvokeGeneratorConfigurationOptions.None, string[]? excludedNames = null, IReadOnlyDictionary<string, string>? remappedNames = null, IReadOnlyDictionary<string, AccessSpecifier>? withAccessSpecifiers = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withAttributes = null, IReadOnlyDictionary<string, string>? withCallConvs = null, IReadOnlyDictionary<string, string>? withClasses = null, IReadOnlyDictionary<string, string>? withLibraryPaths = null, IReadOnlyDictionary<string, string>? withNamespaces = null, string[]? withSetLastErrors = null, IReadOnlyDictionary<string, (string, PInvokeGeneratorTransparentStructKind)>? withTransparentStructs = null, IReadOnlyDictionary<string, string>? withTypes = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withUsings = null, IReadOnlyDictionary<string, string>? withPackings = null, IEnumerable<Diagnostic>? expectedDiagnostics = null, string libraryPath = DefaultLibraryPath, string[]? commandLineArgs = null, string language = "c++", string languageStandard = DefaultCppStandard)
        => ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorOutputMode.Xml, PInvokeGeneratorConfigurationOptions.GenerateCompatibleCode | additionalConfigOptions, excludedNames, remappedNames, withAccessSpecifiers, withAttributes, withCallConvs, withClasses, withLibraryPaths, withNamespaces, withSetLastErrors, withTransparentStructs, withTypes, withUsings, withPackings, expectedDiagnostics, libraryPath, commandLineArgs, language, languageStandard);

    protected static Task ValidateGeneratedXmlCompatibleUnixBindingsAsync(string inputContents, string expectedOutputContents, PInvokeGeneratorConfigurationOptions additionalConfigOptions = PInvokeGeneratorConfigurationOptions.None, string[]? excludedNames = null, IReadOnlyDictionary<string, string>? remappedNames = null, IReadOnlyDictionary<string, AccessSpecifier>? withAccessSpecifiers = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withAttributes = null, IReadOnlyDictionary<string, string>? withCallConvs = null, IReadOnlyDictionary<string, string>? withClasses = null, IReadOnlyDictionary<string, string>? withLibraryPaths = null, IReadOnlyDictionary<string, string>? withNamespaces = null, string[]? withSetLastErrors = null, IReadOnlyDictionary<string, (string, PInvokeGeneratorTransparentStructKind)>? withTransparentStructs = null, IReadOnlyDictionary<string, string>? withTypes = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withUsings = null, IReadOnlyDictionary<string, string>? withPackings = null, IEnumerable<Diagnostic>? expectedDiagnostics = null, string libraryPath = DefaultLibraryPath, string[]? commandLineArgs = null, string language = "c++", string languageStandard = DefaultCppStandard)
        => ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorOutputMode.Xml, PInvokeGeneratorConfigurationOptions.GenerateCompatibleCode | PInvokeGeneratorConfigurationOptions.GenerateUnixTypes | additionalConfigOptions, excludedNames, remappedNames, withAccessSpecifiers, withAttributes, withCallConvs, withClasses, withLibraryPaths, withNamespaces, withSetLastErrors, withTransparentStructs, withTypes, withUsings, withPackings, expectedDiagnostics, libraryPath, commandLineArgs, language, languageStandard);

    private static async Task ValidateGeneratedBindingsAsync(string inputContents, string expectedOutputContents, PInvokeGeneratorOutputMode outputMode, PInvokeGeneratorConfigurationOptions configOptions, string[]? excludedNames, IReadOnlyDictionary<string, string>? remappedNames, IReadOnlyDictionary<string, AccessSpecifier>? withAccessSpecifiers, IReadOnlyDictionary<string, IReadOnlyList<string>>? withAttributes, IReadOnlyDictionary<string, string>? withCallConvs, IReadOnlyDictionary<string, string>? withClasses, IReadOnlyDictionary<string, string>? withLibraryPaths, IReadOnlyDictionary<string, string>? withNamespaces, string[]? withSetLastErrors, IReadOnlyDictionary<string, (string, PInvokeGeneratorTransparentStructKind)>? withTransparentStructs, IReadOnlyDictionary<string, string>? withTypes, IReadOnlyDictionary<string, IReadOnlyList<string>>? withUsings, IReadOnlyDictionary<string, string>? withPackings, IEnumerable<Diagnostic>? expectedDiagnostics, string libraryPath, string[]? commandLineArgs, string language, string languageStandard)
    {
        Assert.That(DefaultInputFileName, Does.Exist);
        commandLineArgs ??= DefaultCppClangCommandLineArgs;

        configOptions |= PInvokeGeneratorConfigurationOptions.GenerateMacroBindings;

        using var outputStream = new MemoryStream();
        using var unsavedFile = CXUnsavedFile.Create(DefaultInputFileName, inputContents);

        var unsavedFiles = new CXUnsavedFile[] { unsavedFile };
        var config = new PInvokeGeneratorConfiguration(language, languageStandard, DefaultNamespaceName, Path.GetRandomFileName(), headerFile: null, outputMode, configOptions) {
            DefaultClass = null,
            ExcludedNames = excludedNames,
            IncludedNames = null,
            LibraryPath = libraryPath,
            MethodPrefixToStrip = null,
            RemappedNames = remappedNames,
            TraversalNames = null,
            TestOutputLocation = null,
            WithAccessSpecifiers = withAccessSpecifiers,
            WithAttributes = withAttributes,
            WithCallConvs = withCallConvs,
            WithClasses = withClasses,
            WithLibraryPaths = withLibraryPaths,
            WithManualImports = null,
            WithNamespaces = withNamespaces,
            WithSetLastErrors = withSetLastErrors,
            WithSuppressGCTransitions = null,
            WithTransparentStructs = withTransparentStructs,
            WithTypes = withTypes,
            WithUsings = withUsings,
            WithPackings = withPackings,
        };

        using (var pinvokeGenerator = new PInvokeGenerator(config, (path) => outputStream))
        {
            var handle = CXTranslationUnit.Parse(pinvokeGenerator.IndexHandle, DefaultInputFileName, commandLineArgs, unsavedFiles, DefaultTranslationUnitFlags);

            using var translationUnit = TranslationUnit.GetOrCreate(handle);
            Debug.Assert(translationUnit is not null);

            pinvokeGenerator.GenerateBindings(translationUnit, DefaultInputFileName, commandLineArgs, DefaultTranslationUnitFlags);

            if (expectedDiagnostics is null)
            {
                Assert.That(pinvokeGenerator.Diagnostics, Is.Empty);
            }
            else
            {
                Assert.That(pinvokeGenerator.Diagnostics, Is.EqualTo(expectedDiagnostics));
            }
        }
        outputStream.Position = 0;

        using var streamReader = new StreamReader(outputStream);
        var actualOutputContents = await streamReader.ReadToEndAsync().ConfigureAwait(false);
        Assert.That(actualOutputContents, Is.EqualTo(expectedOutputContents));
        if (outputMode == PInvokeGeneratorOutputMode.Xml && !string.IsNullOrEmpty(actualOutputContents))
        {
            ValidateRoundTrippedSerializedXmlBindings(actualOutputContents);
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Trimming",
        "IL2026: Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code",
        Justification = nameof(System.Xml.Serialization))]
    private static void ValidateRoundTrippedSerializedXmlBindings(string generatedXmlOutput)
    {
        var generatedNsXml = GetNamespaceXml(generatedXmlOutput, true);

        var bindingsSerializer = new XmlSerializer(typeof(BindingsElement));
        XmlReaderSettings xmlReaderSettings = new() {
            DtdProcessing = DtdProcessing.Ignore,
            IgnoreWhitespace = false,
        };
        using var generatedXmlTextReader = new StringReader(generatedXmlOutput);
        using var generatedXmlReader = XmlReader.Create(generatedXmlTextReader, xmlReaderSettings);
        var deserializedInstance = bindingsSerializer.Deserialize(generatedXmlReader);
        Assert.That(deserializedInstance, Is.Not.Null);
        Assert.That(deserializedInstance, Is.InstanceOf<BindingsElement>());

        var serialiedXmlOutputBuilder = new StringBuilder();
        XmlWriterSettings xmlWriterSettings = new() {
            OmitXmlDeclaration = true,
            Indent = true,
            IndentChars = new(' ', 2),
            NewLineChars = "\n",
        };
        using var serializedXmlWriter = XmlWriter.Create(serialiedXmlOutputBuilder, xmlWriterSettings);
        bindingsSerializer.Serialize(serializedXmlWriter, deserializedInstance);
        serializedXmlWriter.Close();
        var serializedNsXml = GetNamespaceXml(serialiedXmlOutputBuilder.ToString());

        Assert.That(serializedNsXml, Is.EqualTo(generatedNsXml));

        static string GetNamespaceXml(string xmlText, bool normalize = false)
        {
            const string NamespaceBegin = "<namespace";
            const string NamespaceEnd = "/namespace>";
            const string NamespaceSelfClosed = "namespace />";
            var nsStartIndex = xmlText.IndexOf(NamespaceBegin, StringComparison.Ordinal);
            var nsEndIndex = xmlText.LastIndexOf(NamespaceEnd, StringComparison.Ordinal);
            if (nsEndIndex < 0)
            {
                nsEndIndex = xmlText.LastIndexOf(NamespaceSelfClosed, StringComparison.Ordinal);
                if (nsEndIndex < 0)
                {
                    return string.Empty;
                }
                else
                {
                    nsEndIndex += NamespaceSelfClosed.Length;
                }
            }
            else
            {
                nsEndIndex += NamespaceEnd.Length;
            }
            var xmlNsText = xmlText[nsStartIndex..nsEndIndex];
            if (!normalize)
            {
                return xmlNsText;
            }

            var emptyRegex = GetEmptyXmlTagRegex();
            xmlNsText = emptyRegex.Replace(xmlNsText, static emptyMatch =>
            {
                var tagName = emptyMatch.Groups[1];
                var attrs = emptyMatch.Groups[2];
                return $"<{tagName}{attrs} />";
            });
            return xmlNsText;
        }
    }

    [GeneratedRegex("""<(\w+)(\s*[^>]*)></\1>""")]
    private static partial Regex GetEmptyXmlTagRegex();
}
