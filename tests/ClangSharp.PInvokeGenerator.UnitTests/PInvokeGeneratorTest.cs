// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Xml.Linq;
using ClangSharp.Interop;
using Xunit;

namespace ClangSharp.UnitTests
{
    public abstract class PInvokeGeneratorTest
    {
        protected const string DefaultInputFileName = "ClangUnsavedFile.h";
        protected const string DefaultLibraryPath = "ClangSharpPInvokeGenerator";
        protected const string DefaultNamespaceName = "ClangSharp.Test";

        protected const CXTranslationUnit_Flags DefaultTranslationUnitFlags = CXTranslationUnit_Flags.CXTranslationUnit_IncludeAttributedTypes          // Include attributed types in CXType
                                                                            | CXTranslationUnit_Flags.CXTranslationUnit_VisitImplicitAttributes         // Implicit attributes should be visited
                                                                            | CXTranslationUnit_Flags.CXTranslationUnit_DetailedPreprocessingRecord;

        protected static readonly string[] DefaultClangCommandLineArgs = new string[]
        {
            "-std=c++17",                           // The input files should be compiled for C++ 17
            "-xc++",                                // The input files are C++
            "-Wno-pragma-once-outside-header",      // We are processing files which may be header files
            "-Wno-c++11-narrowing"
        };

        protected string EscapeXml(string value)
        {
            return new XText(value).ToString();
        }

        protected Task ValidateGeneratedCSharpLatestWindowsBindingsAsync(string inputContents, string expectedOutputContents, string[] excludedNames = null, IReadOnlyDictionary<string, string> remappedNames = null, IReadOnlyDictionary<string, IReadOnlyList<string>> withAttributes = null, IReadOnlyDictionary<string, string> withCallConvs = null, IReadOnlyDictionary<string, string> withLibraryPaths = null, string[] withSetLastErrors = null, IReadOnlyDictionary<string, string> withTypes = null, IReadOnlyDictionary<string, IReadOnlyList<string>> withUsings = null, IEnumerable<Diagnostic> expectedDiagnostics = null, string libraryPath = DefaultLibraryPath, string[] commandlineArgs = null)
        {
            return ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorOutputMode.CSharp, PInvokeGeneratorConfigurationOptions.None, excludedNames, remappedNames, withAttributes, withCallConvs, withLibraryPaths, withSetLastErrors, withTypes, withUsings, expectedDiagnostics, libraryPath, commandlineArgs);
        }

        protected Task ValidateGeneratedCSharpLatestUnixBindingsAsync(string inputContents, string expectedOutputContents, string[] excludedNames = null, IReadOnlyDictionary<string, string> remappedNames = null, IReadOnlyDictionary<string, IReadOnlyList<string>> withAttributes = null, IReadOnlyDictionary<string, string> withCallConvs = null, IReadOnlyDictionary<string, string> withLibraryPaths = null, string[] withSetLastErrors = null, IReadOnlyDictionary<string, string> withTypes = null, IReadOnlyDictionary<string, IReadOnlyList<string>> withUsings = null, IEnumerable<Diagnostic> expectedDiagnostics = null, string libraryPath = DefaultLibraryPath, string[] commandlineArgs = null)
        {
            return ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorOutputMode.CSharp, PInvokeGeneratorConfigurationOptions.GenerateUnixTypes, excludedNames, remappedNames, withAttributes, withCallConvs, withLibraryPaths, withSetLastErrors, withTypes, withUsings, expectedDiagnostics, libraryPath, commandlineArgs);
        }

        protected Task ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(string inputContents, string expectedOutputContents, string[] excludedNames = null, IReadOnlyDictionary<string, string> remappedNames = null, IReadOnlyDictionary<string, IReadOnlyList<string>> withAttributes = null, IReadOnlyDictionary<string, string> withCallConvs = null, IReadOnlyDictionary<string, string> withLibraryPaths = null, string[] withSetLastErrors = null, IReadOnlyDictionary<string, string> withTypes = null, IReadOnlyDictionary<string, IReadOnlyList<string>> withUsings = null, IEnumerable<Diagnostic> expectedDiagnostics = null, string libraryPath = DefaultLibraryPath, string[] commandlineArgs = null)
        {
            return ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorOutputMode.CSharp, PInvokeGeneratorConfigurationOptions.GenerateCompatibleCode, excludedNames, remappedNames, withAttributes, withCallConvs, withLibraryPaths, withSetLastErrors, withTypes, withUsings, expectedDiagnostics, libraryPath, commandlineArgs);
        }

        protected Task ValidateGeneratedCSharpCompatibleUnixBindingsAsync(string inputContents, string expectedOutputContents, string[] excludedNames = null, IReadOnlyDictionary<string, string> remappedNames = null, IReadOnlyDictionary<string, IReadOnlyList<string>> withAttributes = null, IReadOnlyDictionary<string, string> withCallConvs = null, IReadOnlyDictionary<string, string> withLibraryPaths = null, string[] withSetLastErrors = null, IReadOnlyDictionary<string, string> withTypes = null, IReadOnlyDictionary<string, IReadOnlyList<string>> withUsings = null, IEnumerable<Diagnostic> expectedDiagnostics = null, string libraryPath = DefaultLibraryPath, string[] commandlineArgs = null)
        {
            return ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorOutputMode.CSharp, PInvokeGeneratorConfigurationOptions.GenerateCompatibleCode | PInvokeGeneratorConfigurationOptions.GenerateUnixTypes, excludedNames, remappedNames, withAttributes, withCallConvs, withLibraryPaths, withSetLastErrors, withTypes, withUsings, expectedDiagnostics, libraryPath, commandlineArgs);
        }

        protected Task ValidateGeneratedXmlLatestWindowsBindingsAsync(string inputContents, string expectedOutputContents, string[] excludedNames = null, IReadOnlyDictionary<string, string> remappedNames = null, IReadOnlyDictionary<string, IReadOnlyList<string>> withAttributes = null, IReadOnlyDictionary<string, string> withCallConvs = null, IReadOnlyDictionary<string, string> withLibraryPaths = null, string[] withSetLastErrors = null, IReadOnlyDictionary<string, string> withTypes = null, IReadOnlyDictionary<string, IReadOnlyList<string>> withUsings = null, IEnumerable<Diagnostic> expectedDiagnostics = null, string libraryPath = DefaultLibraryPath, string[] commandlineArgs = null, [CallerFilePath] string filePath = "")
        {
            return ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorOutputMode.Xml, PInvokeGeneratorConfigurationOptions.None, excludedNames, remappedNames, withAttributes, withCallConvs, withLibraryPaths, withSetLastErrors, withTypes, withUsings, expectedDiagnostics, libraryPath, commandlineArgs, filePath);
        }

        protected Task ValidateGeneratedXmlLatestUnixBindingsAsync(string inputContents, string expectedOutputContents, string[] excludedNames = null, IReadOnlyDictionary<string, string> remappedNames = null, IReadOnlyDictionary<string, IReadOnlyList<string>> withAttributes = null, IReadOnlyDictionary<string, string> withCallConvs = null, IReadOnlyDictionary<string, string> withLibraryPaths = null, string[] withSetLastErrors = null, IReadOnlyDictionary<string, string> withTypes = null, IReadOnlyDictionary<string, IReadOnlyList<string>> withUsings = null, IEnumerable<Diagnostic> expectedDiagnostics = null, string libraryPath = DefaultLibraryPath, string[] commandlineArgs = null)
        {
            return ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorOutputMode.Xml, PInvokeGeneratorConfigurationOptions.GenerateUnixTypes, excludedNames, remappedNames, withAttributes, withCallConvs, withLibraryPaths, withSetLastErrors, withTypes, withUsings, expectedDiagnostics, libraryPath, commandlineArgs);
        }

        protected Task ValidateGeneratedXmlCompatibleWindowsBindingsAsync(string inputContents, string expectedOutputContents, string[] excludedNames = null, IReadOnlyDictionary<string, string> remappedNames = null, IReadOnlyDictionary<string, IReadOnlyList<string>> withAttributes = null, IReadOnlyDictionary<string, string> withCallConvs = null, IReadOnlyDictionary<string, string> withLibraryPaths = null, string[] withSetLastErrors = null, IReadOnlyDictionary<string, string> withTypes = null, IReadOnlyDictionary<string, IReadOnlyList<string>> withUsings = null, IEnumerable<Diagnostic> expectedDiagnostics = null, string libraryPath = DefaultLibraryPath, string[] commandlineArgs = null)
        {
            return ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorOutputMode.Xml, PInvokeGeneratorConfigurationOptions.GenerateCompatibleCode, excludedNames, remappedNames, withAttributes, withCallConvs, withLibraryPaths, withSetLastErrors, withTypes, withUsings, expectedDiagnostics, libraryPath, commandlineArgs);
        }

        protected Task ValidateGeneratedXmlCompatibleUnixBindingsAsync(string inputContents, string expectedOutputContents, string[] excludedNames = null, IReadOnlyDictionary<string, string> remappedNames = null, IReadOnlyDictionary<string, IReadOnlyList<string>> withAttributes = null, IReadOnlyDictionary<string, string> withCallConvs = null, IReadOnlyDictionary<string, string> withLibraryPaths = null, string[] withSetLastErrors = null, IReadOnlyDictionary<string, string> withTypes = null, IReadOnlyDictionary<string, IReadOnlyList<string>> withUsings = null, IEnumerable<Diagnostic> expectedDiagnostics = null, string libraryPath = DefaultLibraryPath, string[] commandlineArgs = null)
        {
            return ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorOutputMode.Xml, PInvokeGeneratorConfigurationOptions.GenerateCompatibleCode | PInvokeGeneratorConfigurationOptions.GenerateUnixTypes, excludedNames, remappedNames, withAttributes, withCallConvs, withLibraryPaths, withSetLastErrors, withTypes, withUsings, expectedDiagnostics, libraryPath, commandlineArgs);
        }

        private async Task ValidateGeneratedBindingsAsync(string inputContents, string expectedOutputContents, PInvokeGeneratorOutputMode outputMode, PInvokeGeneratorConfigurationOptions configOptions, string[] excludedNames, IReadOnlyDictionary<string, string> remappedNames, IReadOnlyDictionary<string, IReadOnlyList<string>> withAttributes, IReadOnlyDictionary<string, string> withCallConvs, IReadOnlyDictionary<string, string> withLibraryPaths, string[] withSetLastErrors, IReadOnlyDictionary<string, string> withTypes, IReadOnlyDictionary<string, IReadOnlyList<string>> withUsings, IEnumerable<Diagnostic> expectedDiagnostics, string libraryPath, string[] commandlineArgs, [CallerFilePath] string filePath = "")
        {
            Assert.True(File.Exists(DefaultInputFileName));

            commandlineArgs ??= DefaultClangCommandLineArgs;

            configOptions |= PInvokeGeneratorConfigurationOptions.GenerateMacroBindings;

            using var outputStream = new MemoryStream();
            using var unsavedFile = CXUnsavedFile.Create(DefaultInputFileName, inputContents);

            var unsavedFiles = new CXUnsavedFile[] { unsavedFile };
            var config = new PInvokeGeneratorConfiguration(libraryPath, DefaultNamespaceName, Path.GetRandomFileName(), testOutputLocation: null, outputMode, configOptions, excludedNames, headerFile: null, methodClassName: null, methodPrefixToStrip: null, remappedNames, traversalNames: null, withAttributes, withCallConvs, withLibraryPaths, withSetLastErrors, withTypes, withUsings);

            using (var pinvokeGenerator = new PInvokeGenerator(config, (path) => outputStream))
            {
                var handle = CXTranslationUnit.Parse(pinvokeGenerator.IndexHandle, DefaultInputFileName, commandlineArgs, unsavedFiles, DefaultTranslationUnitFlags);
                using var translationUnit = TranslationUnit.GetOrCreate(handle);

                pinvokeGenerator.GenerateBindings(translationUnit, DefaultInputFileName, commandlineArgs, DefaultTranslationUnitFlags);

                if (expectedDiagnostics is null)
                {
                    Assert.Empty(pinvokeGenerator.Diagnostics);
                }
                else
                {
                    Assert.Equal(expectedDiagnostics, pinvokeGenerator.Diagnostics);
                }
            }
            outputStream.Position = 0;

            var actualOutputContents = await new StreamReader(outputStream).ReadToEndAsync();
            Assert.Equal(expectedOutputContents, actualOutputContents);
        }
    }
}
