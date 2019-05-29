using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace ClangSharp.Test
{
    public abstract class PInvokeGeneratorTest
    {
        protected const string DefaultInputFileName = "ClangUnsavedFile.h";
        protected const string DefaultLibraryPath = "ClangSharpPInvokeGenerator";
        protected const string DefaultNamespaceName = "ClangSharp.Test";

        protected const CXTranslationUnit_Flags DefaultTranslationUnitFlags = CXTranslationUnit_Flags.CXTranslationUnit_SkipFunctionBodies          // Don't traverse function bodies
                                                                            | CXTranslationUnit_Flags.CXTranslationUnit_IncludeAttributedTypes      // Include attributed types in CXType
                                                                            | CXTranslationUnit_Flags.CXTranslationUnit_VisitImplicitAttributes;    // Implicit attributes should be visited

        protected static readonly string[] DefaultClangCommandLineArgs = new string[]
        {
            "-xc++",                                // The input files are C++
            "-Wno-pragma-once-outside-header"       // We are processing files which may be header files
        };

        protected Task ValidateGeneratedBindings(string inputContents, string expectedOutputContents, params string[] excludedNames)
        {
            return ValidateGeneratedBindings(inputContents, expectedOutputContents, PInvokeGeneratorConfigurationOptions.None, excludedNames);
        }

        protected Task ValidateUnsafeGeneratedBindings(string inputContents, string expectedOutputContents, params string[] excludedNames)
        {
            return ValidateGeneratedBindings(inputContents, expectedOutputContents, PInvokeGeneratorConfigurationOptions.GenerateUnsafeCode, excludedNames);
        }

        private async Task ValidateGeneratedBindings(string inputContents, string expectedOutputContents, PInvokeGeneratorConfigurationOptions configOptions, string[] excludedNames)
        {
            using (var outputStream = new MemoryStream())
            {
                var unsavedInputFile = CXUnsavedFile.Create(DefaultInputFileName, inputContents);
                var config = new PInvokeGeneratorConfiguration(DefaultLibraryPath, DefaultNamespaceName, Path.GetRandomFileName(), configOptions, excludedNames);

                using (var pinvokeGenerator = new PInvokeGenerator(config, ((path) => (outputStream, leaveOpen: true))))
                using (var translationUnitHandle = CXTranslationUnit.Parse(pinvokeGenerator.IndexHandle, DefaultInputFileName, DefaultClangCommandLineArgs, new CXUnsavedFile[] { unsavedInputFile }, DefaultTranslationUnitFlags))
                {
                    pinvokeGenerator.GenerateBindings(translationUnitHandle);
                    Assert.Empty(pinvokeGenerator.Diagnostics);
                }

                outputStream.Position = 0;
                var actualOutputContents = await new StreamReader(outputStream).ReadToEndAsync();
                Assert.Equal(expectedOutputContents, actualOutputContents);
            }
        }
    }
}
