using System;
using System.IO;
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
            // Ideally we would also have the input file be a MemoryStream. Unfortunately,
            // Clang currently requires that all files exist on disk before Parse is called.

            using (var inputFile = new TemporaryFile())
            using (var outputStream = new MemoryStream())
            {
                await inputFile.WriteAllText(inputContents);
                var config = new PInvokeGeneratorConfiguration(DefaultLibraryPath, DefaultNamespaceName, Path.GetRandomFileName());

                using (var pinvokeGenerator = new PInvokeGenerator(config, ((path) => (outputStream, leaveOpen: true))))
                using (var translationUnitHandle = CXTranslationUnit.Parse(pinvokeGenerator.IndexHandle, inputFile.Path, DefaultClangCommandLineArgs, Array.Empty<CXUnsavedFile>(), DefaultTranslationUnitFlags))
                {
                    pinvokeGenerator.GenerateBindings(translationUnitHandle);
                }

                outputStream.Position = 0;
                var actualOutputContents = await new StreamReader(outputStream).ReadToEndAsync();
                Assert.Equal(expectedOutputContents, actualOutputContents);
            }
        }
    }
}
