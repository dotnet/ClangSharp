using System;
using System.Threading.Tasks;
using Xunit;

namespace ClangSharp.Test
{
    public abstract class PInvokeGeneratorTest
    {
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

        protected async Task ValidateGeneratedBindings(string inputContents, string expectedOutputContents)
        {
            using (var inputFile = new TemporaryFile())
            using (var outputFile = new TemporaryFile())
            {
                await inputFile.WriteAllText(inputContents);
                var config = new PInvokeGeneratorConfiguration(DefaultLibraryPath, DefaultNamespaceName, outputFile.Path);

                using (var pinvokeGenerator = new PInvokeGenerator(config))
                using (var translationUnitHandle = CXTranslationUnit.Parse(pinvokeGenerator.IndexHandle, inputFile.Path, DefaultClangCommandLineArgs, Array.Empty<CXUnsavedFile>(), DefaultTranslationUnitFlags))
                {
                    pinvokeGenerator.GenerateBindings(translationUnitHandle);
                }

                var actualOutputContents = await outputFile.ReadAllText();
                Assert.Equal(expectedOutputContents, actualOutputContents);
            }
        }
    }
}
