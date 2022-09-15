// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Xml.Linq;
using ClangSharp.Abstractions;
using ClangSharp.Interop;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

public abstract class PInvokeGeneratorTest
{
    protected const string DefaultInputFileName = "ClangUnsavedFile.h";
    protected const string DefaultLibraryPath = "ClangSharpPInvokeGenerator";
    protected const string DefaultNamespaceName = "ClangSharp.Test";

    protected const CXTranslationUnit_Flags DefaultTranslationUnitFlags = CXTranslationUnit_Flags.CXTranslationUnit_IncludeAttributedTypes          // Include attributed types in CXType
                                                                        | CXTranslationUnit_Flags.CXTranslationUnit_VisitImplicitAttributes         // Implicit attributes should be visited
                                                                        | CXTranslationUnit_Flags.CXTranslationUnit_DetailedPreprocessingRecord;

    protected static readonly string[] DefaultCClangCommandLineArgs = new string[]
    {
        "-std=c17",                             // The input files should be compiled for C 17
        "-xc",                                  // The input files are C
    };

    protected static readonly string[] DefaultCppClangCommandLineArgs = new string[]
    {
        "-std=c++17",                           // The input files should be compiled for C++ 17
        "-xc++",                                // The input files are C++
        "-Wno-pragma-once-outside-header",      // We are processing files which may be header files
        "-Wno-c++11-narrowing"
    };

    protected static string EscapeXml(string value) => new XText(value).ToString();

    protected static Task ValidateGeneratedCSharpPreviewWindowsBindingsAsync(string inputContents, string expectedOutputContents, PInvokeGeneratorConfigurationOptions additionalConfigOptions = PInvokeGeneratorConfigurationOptions.None, string[] excludedNames = null, IReadOnlyDictionary<string, string> remappedNames = null, IReadOnlyDictionary<string, AccessSpecifier> withAccessSpecifiers = null, IReadOnlyDictionary<string, IReadOnlyList<string>> withAttributes = null, IReadOnlyDictionary<string, string> withCallConvs = null, IReadOnlyDictionary<string, string> withClasses = null, IReadOnlyDictionary<string, string> withLibraryPaths = null, IReadOnlyDictionary<string, string> withNamespaces = null, string[] withSetLastErrors = null, IReadOnlyDictionary<string, (string, PInvokeGeneratorTransparentStructKind)> withTransparentStructs = null, IReadOnlyDictionary<string, string> withTypes = null, IReadOnlyDictionary<string, IReadOnlyList<string>> withUsings = null, IEnumerable<Diagnostic> expectedDiagnostics = null, string libraryPath = DefaultLibraryPath, string[] commandlineArgs = null)
        => ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorOutputMode.CSharp, PInvokeGeneratorConfigurationOptions.GeneratePreviewCode | PInvokeGeneratorConfigurationOptions.None | additionalConfigOptions, excludedNames, remappedNames, withAccessSpecifiers, withAttributes, withCallConvs, withClasses, withLibraryPaths, withNamespaces, withSetLastErrors, withTransparentStructs, withTypes, withUsings, expectedDiagnostics, libraryPath, commandlineArgs);

    protected static Task ValidateGeneratedCSharpPreviewUnixBindingsAsync(string inputContents, string expectedOutputContents, PInvokeGeneratorConfigurationOptions additionalConfigOptions = PInvokeGeneratorConfigurationOptions.None, string[] excludedNames = null, IReadOnlyDictionary<string, string> remappedNames = null, IReadOnlyDictionary<string, AccessSpecifier> withAccessSpecifiers = null, IReadOnlyDictionary<string, IReadOnlyList<string>> withAttributes = null, IReadOnlyDictionary<string, string> withCallConvs = null, IReadOnlyDictionary<string, string> withClasses = null, IReadOnlyDictionary<string, string> withLibraryPaths = null, IReadOnlyDictionary<string, string> withNamespaces = null, string[] withSetLastErrors = null, IReadOnlyDictionary<string, (string, PInvokeGeneratorTransparentStructKind)> withTransparentStructs = null, IReadOnlyDictionary<string, string> withTypes = null, IReadOnlyDictionary<string, IReadOnlyList<string>> withUsings = null, IEnumerable<Diagnostic> expectedDiagnostics = null, string libraryPath = DefaultLibraryPath, string[] commandlineArgs = null)
        => ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorOutputMode.CSharp, PInvokeGeneratorConfigurationOptions.GeneratePreviewCode | PInvokeGeneratorConfigurationOptions.GenerateUnixTypes | additionalConfigOptions, excludedNames, remappedNames, withAccessSpecifiers, withAttributes, withCallConvs, withClasses, withLibraryPaths, withNamespaces, withSetLastErrors, withTransparentStructs, withTypes, withUsings, expectedDiagnostics, libraryPath, commandlineArgs);

    protected static Task ValidateGeneratedCSharpLatestWindowsBindingsAsync(string inputContents, string expectedOutputContents, PInvokeGeneratorConfigurationOptions additionalConfigOptions = PInvokeGeneratorConfigurationOptions.None, string[] excludedNames = null, IReadOnlyDictionary<string, string> remappedNames = null, IReadOnlyDictionary<string, AccessSpecifier> withAccessSpecifiers = null, IReadOnlyDictionary<string, IReadOnlyList<string>> withAttributes = null, IReadOnlyDictionary<string, string> withCallConvs = null, IReadOnlyDictionary<string, string> withClasses = null, IReadOnlyDictionary<string, string> withLibraryPaths = null, IReadOnlyDictionary<string, string> withNamespaces = null, string[] withSetLastErrors = null, IReadOnlyDictionary<string, (string, PInvokeGeneratorTransparentStructKind)> withTransparentStructs = null, IReadOnlyDictionary<string, string> withTypes = null, IReadOnlyDictionary<string, IReadOnlyList<string>> withUsings = null, IEnumerable<Diagnostic> expectedDiagnostics = null, string libraryPath = DefaultLibraryPath, string[] commandlineArgs = null)
        => ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorOutputMode.CSharp, PInvokeGeneratorConfigurationOptions.None | additionalConfigOptions, excludedNames, remappedNames, withAccessSpecifiers, withAttributes, withCallConvs, withClasses, withLibraryPaths, withNamespaces, withSetLastErrors, withTransparentStructs, withTypes, withUsings, expectedDiagnostics, libraryPath, commandlineArgs);

    protected static Task ValidateGeneratedCSharpLatestUnixBindingsAsync(string inputContents, string expectedOutputContents, PInvokeGeneratorConfigurationOptions additionalConfigOptions = PInvokeGeneratorConfigurationOptions.None, string[] excludedNames = null, IReadOnlyDictionary<string, string> remappedNames = null, IReadOnlyDictionary<string, AccessSpecifier> withAccessSpecifiers = null, IReadOnlyDictionary<string, IReadOnlyList<string>> withAttributes = null, IReadOnlyDictionary<string, string> withCallConvs = null, IReadOnlyDictionary<string, string> withClasses = null, IReadOnlyDictionary<string, string> withLibraryPaths = null, IReadOnlyDictionary<string, string> withNamespaces = null, string[] withSetLastErrors = null, IReadOnlyDictionary<string, (string, PInvokeGeneratorTransparentStructKind)> withTransparentStructs = null, IReadOnlyDictionary<string, string> withTypes = null, IReadOnlyDictionary<string, IReadOnlyList<string>> withUsings = null, IEnumerable<Diagnostic> expectedDiagnostics = null, string libraryPath = DefaultLibraryPath, string[] commandlineArgs = null)
        => ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorOutputMode.CSharp, PInvokeGeneratorConfigurationOptions.GenerateUnixTypes | additionalConfigOptions, excludedNames, remappedNames, withAccessSpecifiers, withAttributes, withCallConvs, withClasses, withLibraryPaths, withNamespaces, withSetLastErrors, withTransparentStructs, withTypes, withUsings, expectedDiagnostics, libraryPath, commandlineArgs);

    protected static Task ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(string inputContents, string expectedOutputContents, PInvokeGeneratorConfigurationOptions additionalConfigOptions = PInvokeGeneratorConfigurationOptions.None, string[] excludedNames = null, IReadOnlyDictionary<string, string> remappedNames = null, IReadOnlyDictionary<string, AccessSpecifier> withAccessSpecifiers = null, IReadOnlyDictionary<string, IReadOnlyList<string>> withAttributes = null, IReadOnlyDictionary<string, string> withCallConvs = null, IReadOnlyDictionary<string, string> withClasses = null, IReadOnlyDictionary<string, string> withLibraryPaths = null, IReadOnlyDictionary<string, string> withNamespaces = null, string[] withSetLastErrors = null, IReadOnlyDictionary<string, (string, PInvokeGeneratorTransparentStructKind)> withTransparentStructs = null, IReadOnlyDictionary<string, string> withTypes = null, IReadOnlyDictionary<string, IReadOnlyList<string>> withUsings = null, IEnumerable<Diagnostic> expectedDiagnostics = null, string libraryPath = DefaultLibraryPath, string[] commandlineArgs = null)
        => ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorOutputMode.CSharp, PInvokeGeneratorConfigurationOptions.GenerateCompatibleCode | additionalConfigOptions, excludedNames, remappedNames, withAccessSpecifiers, withAttributes, withCallConvs, withClasses, withLibraryPaths, withNamespaces, withSetLastErrors, withTransparentStructs, withTypes, withUsings, expectedDiagnostics, libraryPath, commandlineArgs);

    protected static Task ValidateGeneratedCSharpCompatibleUnixBindingsAsync(string inputContents, string expectedOutputContents, PInvokeGeneratorConfigurationOptions additionalConfigOptions = PInvokeGeneratorConfigurationOptions.None, string[] excludedNames = null, IReadOnlyDictionary<string, string> remappedNames = null, IReadOnlyDictionary<string, AccessSpecifier> withAccessSpecifiers = null, IReadOnlyDictionary<string, IReadOnlyList<string>> withAttributes = null, IReadOnlyDictionary<string, string> withCallConvs = null, IReadOnlyDictionary<string, string> withClasses = null, IReadOnlyDictionary<string, string> withLibraryPaths = null, IReadOnlyDictionary<string, string> withNamespaces = null, string[] withSetLastErrors = null, IReadOnlyDictionary<string, (string, PInvokeGeneratorTransparentStructKind)> withTransparentStructs = null, IReadOnlyDictionary<string, string> withTypes = null, IReadOnlyDictionary<string, IReadOnlyList<string>> withUsings = null, IEnumerable<Diagnostic> expectedDiagnostics = null, string libraryPath = DefaultLibraryPath, string[] commandlineArgs = null)
        => ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorOutputMode.CSharp, PInvokeGeneratorConfigurationOptions.GenerateCompatibleCode | PInvokeGeneratorConfigurationOptions.GenerateUnixTypes | additionalConfigOptions, excludedNames, remappedNames, withAccessSpecifiers, withAttributes, withCallConvs, withClasses, withLibraryPaths, withNamespaces, withSetLastErrors, withTransparentStructs, withTypes, withUsings, expectedDiagnostics, libraryPath, commandlineArgs);

    protected static Task ValidateGeneratedXmlPreviewWindowsBindingsAsync(string inputContents, string expectedOutputContents, PInvokeGeneratorConfigurationOptions additionalConfigOptions = PInvokeGeneratorConfigurationOptions.None, string[] excludedNames = null, IReadOnlyDictionary<string, string> remappedNames = null, IReadOnlyDictionary<string, AccessSpecifier> withAccessSpecifiers = null, IReadOnlyDictionary<string, IReadOnlyList<string>> withAttributes = null, IReadOnlyDictionary<string, string> withCallConvs = null, IReadOnlyDictionary<string, string> withClasses = null, IReadOnlyDictionary<string, string> withLibraryPaths = null, IReadOnlyDictionary<string, string> withNamespaces = null, string[] withSetLastErrors = null, IReadOnlyDictionary<string, (string, PInvokeGeneratorTransparentStructKind)> withTransparentStructs = null, IReadOnlyDictionary<string, string> withTypes = null, IReadOnlyDictionary<string, IReadOnlyList<string>> withUsings = null, IEnumerable<Diagnostic> expectedDiagnostics = null, string libraryPath = DefaultLibraryPath, string[] commandlineArgs = null, [CallerFilePath] string filePath = "")
        => ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorOutputMode.Xml, PInvokeGeneratorConfigurationOptions.GeneratePreviewCode | additionalConfigOptions, excludedNames, remappedNames, withAccessSpecifiers, withAttributes, withCallConvs, withClasses, withLibraryPaths, withNamespaces, withSetLastErrors, withTransparentStructs, withTypes, withUsings, expectedDiagnostics, libraryPath, commandlineArgs, filePath);

    protected static Task ValidateGeneratedXmlPreviewUnixBindingsAsync(string inputContents, string expectedOutputContents, PInvokeGeneratorConfigurationOptions additionalConfigOptions = PInvokeGeneratorConfigurationOptions.None, string[] excludedNames = null, IReadOnlyDictionary<string, string> remappedNames = null, IReadOnlyDictionary<string, AccessSpecifier> withAccessSpecifiers = null, IReadOnlyDictionary<string, IReadOnlyList<string>> withAttributes = null, IReadOnlyDictionary<string, string> withCallConvs = null, IReadOnlyDictionary<string, string> withClasses = null, IReadOnlyDictionary<string, string> withLibraryPaths = null, IReadOnlyDictionary<string, string> withNamespaces = null, string[] withSetLastErrors = null, IReadOnlyDictionary<string, (string, PInvokeGeneratorTransparentStructKind)> withTransparentStructs = null, IReadOnlyDictionary<string, string> withTypes = null, IReadOnlyDictionary<string, IReadOnlyList<string>> withUsings = null, IEnumerable<Diagnostic> expectedDiagnostics = null, string libraryPath = DefaultLibraryPath, string[] commandlineArgs = null)
        => ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorOutputMode.Xml, PInvokeGeneratorConfigurationOptions.GeneratePreviewCode | PInvokeGeneratorConfigurationOptions.GenerateUnixTypes | additionalConfigOptions, excludedNames, remappedNames, withAccessSpecifiers, withAttributes, withCallConvs, withClasses, withLibraryPaths, withNamespaces, withSetLastErrors, withTransparentStructs, withTypes, withUsings, expectedDiagnostics, libraryPath, commandlineArgs);

    protected static Task ValidateGeneratedXmlLatestWindowsBindingsAsync(string inputContents, string expectedOutputContents, PInvokeGeneratorConfigurationOptions additionalConfigOptions = PInvokeGeneratorConfigurationOptions.None, string[] excludedNames = null, IReadOnlyDictionary<string, string> remappedNames = null, IReadOnlyDictionary<string, AccessSpecifier> withAccessSpecifiers = null, IReadOnlyDictionary<string, IReadOnlyList<string>> withAttributes = null, IReadOnlyDictionary<string, string> withCallConvs = null, IReadOnlyDictionary<string, string> withClasses = null, IReadOnlyDictionary<string, string> withLibraryPaths = null, IReadOnlyDictionary<string, string> withNamespaces = null, string[] withSetLastErrors = null, IReadOnlyDictionary<string, (string, PInvokeGeneratorTransparentStructKind)> withTransparentStructs = null, IReadOnlyDictionary<string, string> withTypes = null, IReadOnlyDictionary<string, IReadOnlyList<string>> withUsings = null, IEnumerable<Diagnostic> expectedDiagnostics = null, string libraryPath = DefaultLibraryPath, string[] commandlineArgs = null, [CallerFilePath] string filePath = "")
        => ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorOutputMode.Xml, PInvokeGeneratorConfigurationOptions.None | additionalConfigOptions, excludedNames, remappedNames, withAccessSpecifiers, withAttributes, withCallConvs, withClasses, withLibraryPaths, withNamespaces, withSetLastErrors, withTransparentStructs, withTypes, withUsings, expectedDiagnostics, libraryPath, commandlineArgs, filePath);

    protected static Task ValidateGeneratedXmlLatestUnixBindingsAsync(string inputContents, string expectedOutputContents, PInvokeGeneratorConfigurationOptions additionalConfigOptions = PInvokeGeneratorConfigurationOptions.None, string[] excludedNames = null, IReadOnlyDictionary<string, string> remappedNames = null, IReadOnlyDictionary<string, AccessSpecifier> withAccessSpecifiers = null, IReadOnlyDictionary<string, IReadOnlyList<string>> withAttributes = null, IReadOnlyDictionary<string, string> withCallConvs = null, IReadOnlyDictionary<string, string> withClasses = null, IReadOnlyDictionary<string, string> withLibraryPaths = null, IReadOnlyDictionary<string, string> withNamespaces = null, string[] withSetLastErrors = null, IReadOnlyDictionary<string, (string, PInvokeGeneratorTransparentStructKind)> withTransparentStructs = null, IReadOnlyDictionary<string, string> withTypes = null, IReadOnlyDictionary<string, IReadOnlyList<string>> withUsings = null, IEnumerable<Diagnostic> expectedDiagnostics = null, string libraryPath = DefaultLibraryPath, string[] commandlineArgs = null)
        => ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorOutputMode.Xml, PInvokeGeneratorConfigurationOptions.GenerateUnixTypes | additionalConfigOptions, excludedNames, remappedNames, withAccessSpecifiers, withAttributes, withCallConvs, withClasses, withLibraryPaths, withNamespaces, withSetLastErrors, withTransparentStructs, withTypes, withUsings, expectedDiagnostics, libraryPath, commandlineArgs);

    protected static Task ValidateGeneratedXmlCompatibleWindowsBindingsAsync(string inputContents, string expectedOutputContents, PInvokeGeneratorConfigurationOptions additionalConfigOptions = PInvokeGeneratorConfigurationOptions.None, string[] excludedNames = null, IReadOnlyDictionary<string, string> remappedNames = null, IReadOnlyDictionary<string, AccessSpecifier> withAccessSpecifiers = null, IReadOnlyDictionary<string, IReadOnlyList<string>> withAttributes = null, IReadOnlyDictionary<string, string> withCallConvs = null, IReadOnlyDictionary<string, string> withClasses = null, IReadOnlyDictionary<string, string> withLibraryPaths = null, IReadOnlyDictionary<string, string> withNamespaces = null, string[] withSetLastErrors = null, IReadOnlyDictionary<string, (string, PInvokeGeneratorTransparentStructKind)> withTransparentStructs = null, IReadOnlyDictionary<string, string> withTypes = null, IReadOnlyDictionary<string, IReadOnlyList<string>> withUsings = null, IEnumerable<Diagnostic> expectedDiagnostics = null, string libraryPath = DefaultLibraryPath, string[] commandlineArgs = null)
        => ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorOutputMode.Xml, PInvokeGeneratorConfigurationOptions.GenerateCompatibleCode | additionalConfigOptions, excludedNames, remappedNames, withAccessSpecifiers, withAttributes, withCallConvs, withClasses, withLibraryPaths, withNamespaces, withSetLastErrors, withTransparentStructs, withTypes, withUsings, expectedDiagnostics, libraryPath, commandlineArgs);

    protected static Task ValidateGeneratedXmlCompatibleUnixBindingsAsync(string inputContents, string expectedOutputContents, PInvokeGeneratorConfigurationOptions additionalConfigOptions = PInvokeGeneratorConfigurationOptions.None, string[] excludedNames = null, IReadOnlyDictionary<string, string> remappedNames = null, IReadOnlyDictionary<string, AccessSpecifier> withAccessSpecifiers = null, IReadOnlyDictionary<string, IReadOnlyList<string>> withAttributes = null, IReadOnlyDictionary<string, string> withCallConvs = null, IReadOnlyDictionary<string, string> withClasses = null, IReadOnlyDictionary<string, string> withLibraryPaths = null, IReadOnlyDictionary<string, string> withNamespaces = null, string[] withSetLastErrors = null, IReadOnlyDictionary<string, (string, PInvokeGeneratorTransparentStructKind)> withTransparentStructs = null, IReadOnlyDictionary<string, string> withTypes = null, IReadOnlyDictionary<string, IReadOnlyList<string>> withUsings = null, IEnumerable<Diagnostic> expectedDiagnostics = null, string libraryPath = DefaultLibraryPath, string[] commandlineArgs = null)
        => ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorOutputMode.Xml, PInvokeGeneratorConfigurationOptions.GenerateCompatibleCode | PInvokeGeneratorConfigurationOptions.GenerateUnixTypes | additionalConfigOptions, excludedNames, remappedNames, withAccessSpecifiers, withAttributes, withCallConvs, withClasses, withLibraryPaths, withNamespaces, withSetLastErrors, withTransparentStructs, withTypes, withUsings, expectedDiagnostics, libraryPath, commandlineArgs);

    private static async Task ValidateGeneratedBindingsAsync(string inputContents, string expectedOutputContents, PInvokeGeneratorOutputMode outputMode, PInvokeGeneratorConfigurationOptions configOptions, string[] excludedNames, IReadOnlyDictionary<string, string> remappedNames, IReadOnlyDictionary<string, AccessSpecifier> withAccessSpecifiers, IReadOnlyDictionary<string, IReadOnlyList<string>> withAttributes, IReadOnlyDictionary<string, string> withCallConvs, IReadOnlyDictionary<string, string> withClasses, IReadOnlyDictionary<string, string> withLibraryPaths, IReadOnlyDictionary<string, string> withNamespaces, string[] withSetLastErrors, IReadOnlyDictionary<string, (string, PInvokeGeneratorTransparentStructKind)> withTransparentStructs, IReadOnlyDictionary<string, string> withTypes, IReadOnlyDictionary<string, IReadOnlyList<string>> withUsings, IEnumerable<Diagnostic> expectedDiagnostics, string libraryPath, string[] commandlineArgs, [CallerFilePath] string filePath = "")
    {
        Assert.True(File.Exists(DefaultInputFileName));
        commandlineArgs ??= DefaultCppClangCommandLineArgs;

        configOptions |= PInvokeGeneratorConfigurationOptions.GenerateMacroBindings;

        using var outputStream = new MemoryStream();
        using var unsavedFile = CXUnsavedFile.Create(DefaultInputFileName, inputContents);

        var unsavedFiles = new CXUnsavedFile[] { unsavedFile };
        var config = new PInvokeGeneratorConfiguration(DefaultNamespaceName, Path.GetRandomFileName(), headerFile: null, outputMode, configOptions) {
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
        };

        using (var pinvokeGenerator = new PInvokeGenerator(config, (path) => outputStream))
        {
            var handle = CXTranslationUnit.Parse(pinvokeGenerator.IndexHandle, DefaultInputFileName, commandlineArgs, unsavedFiles, DefaultTranslationUnitFlags);
            using var translationUnit = TranslationUnit.GetOrCreate(handle);

            pinvokeGenerator.GenerateBindings(translationUnit, DefaultInputFileName, commandlineArgs, DefaultTranslationUnitFlags);

            if (expectedDiagnostics is null)
            {
                Assert.IsEmpty(pinvokeGenerator.Diagnostics);
            }
            else
            {
                Assert.AreEqual(expectedDiagnostics, pinvokeGenerator.Diagnostics);
            }
        }
        outputStream.Position = 0;

        var actualOutputContents = await new StreamReader(outputStream).ReadToEndAsync();
        Assert.AreEqual(expectedOutputContents, actualOutputContents);
    }
}
