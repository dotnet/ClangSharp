using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace ClangSharp.UnitTests
{
    public abstract class PInvokeGeneratorTest
    {
        protected const string DefaultInputFileName = "ClangUnsavedFile.h";
        protected const string DefaultLibraryPath = "ClangSharpPInvokeGenerator";
        protected const string DefaultNamespaceName = "ClangSharp.Test";

        protected const CXTranslationUnit_Flags DefaultTranslationUnitFlags = CXTranslationUnit_Flags.CXTranslationUnit_IncludeAttributedTypes      // Include attributed types in CXType
                                                                            | CXTranslationUnit_Flags.CXTranslationUnit_VisitImplicitAttributes;    // Implicit attributes should be visited

        protected static readonly string[] DefaultClangCommandLineArgs = new string[]
        {
            "-xc++",                                // The input files are C++
            "-Wno-pragma-once-outside-header"       // We are processing files which may be header files
        };

        protected Task ValidateGeneratedBindings(string inputContents, string expectedOutputContents, string[] excludedNames = null, IReadOnlyDictionary<string, string> remappedNames = null)
        {
            return ValidateGeneratedBindings(inputContents, expectedOutputContents, PInvokeGeneratorConfigurationOptions.None, excludedNames, remappedNames);
        }

        protected Task ValidateUnsafeGeneratedBindings(string inputContents, string expectedOutputContents, string[] excludedNames = null, IReadOnlyDictionary<string, string> remappedNames = null)
        {
            return ValidateGeneratedBindings(inputContents, expectedOutputContents, PInvokeGeneratorConfigurationOptions.GenerateUnsafeCode, excludedNames, remappedNames);
        }

        private async Task ValidateGeneratedBindings(string inputContents, string expectedOutputContents, PInvokeGeneratorConfigurationOptions configOptions, string[] excludedNames, IReadOnlyDictionary<string, string> remappedNames)
        {
            using (var outputStream = new MemoryStream())
            {
                var unsavedInputFile = CXUnsavedFile.Create(DefaultInputFileName, inputContents);
                var config = new PInvokeGeneratorConfiguration(DefaultLibraryPath, DefaultNamespaceName, Path.GetRandomFileName(), configOptions, excludedNames, methodClassName: null, methodPrefixToStrip: null, remappedNames);

                using (var pinvokeGenerator = new PInvokeGenerator(config, (path) => outputStream))
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
