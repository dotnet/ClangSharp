// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.IO;
using ClangSharp.Interop;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

public sealed class PreprocessingRecordTest : PInvokeGeneratorTest
{
    [Test]
    public void DetailedPreprocessingRecordsWithoutMacroBindingsWarnOnceAndContinue()
    {
        const string inputContents = """
            #define FIRST_MACRO 1
            #define SECOND_MACRO 2
            int Example();
            """;

        using var outputStream = new MemoryStream();
        using var unsavedFile = CXUnsavedFile.Create(DefaultInputFileName, inputContents);
        var unsavedFiles = new CXUnsavedFile[] { unsavedFile };
        var config = new PInvokeGeneratorConfiguration("c++", DefaultCppStandard, DefaultNamespaceName, Path.GetRandomFileName(), headerFile: null, PInvokeGeneratorOutputMode.CSharp, PInvokeGeneratorConfigurationOptions.None) {
            DefaultClass = null,
            LibraryPath = DefaultLibraryPath,
        };

        var expectedDiagnostics = new Diagnostic[] {
            new(DiagnosticLevel.Warning, "Preprocessing records were ignored because macro binding generation is not enabled. Enable 'GenerateMacroBindings' ('--generate-macro-bindings' in the CLI) to generate supported macro bindings."),
        };

        using (var generator = new PInvokeGenerator(config, _ => outputStream))
        {
            var handle = CXTranslationUnit.Parse(generator.IndexHandle, DefaultInputFileName, DefaultCppClangCommandLineArgs, unsavedFiles, DefaultTranslationUnitFlags);

            using var translationUnit = TranslationUnit.GetOrCreate(handle);
            generator.GenerateBindings(translationUnit, DefaultInputFileName, DefaultCppClangCommandLineArgs, DefaultTranslationUnitFlags);

            Assert.That(generator.Diagnostics, Is.EqualTo(expectedDiagnostics));
        }

        outputStream.Position = 0;
        using var streamReader = new StreamReader(outputStream);
        var actualOutputContents = streamReader.ReadToEnd();

        Assert.That(actualOutputContents, Does.Contain("Example"));
        Assert.That(actualOutputContents, Does.Not.Contain("FIRST_MACRO"));
        Assert.That(actualOutputContents, Does.Not.Contain("SECOND_MACRO"));
    }
}
