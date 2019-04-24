using System;
using System.IO;
using Xunit;

namespace ClangSharp.Test
{
    // This is not ported from libclangtest but instead created to test Unicode stuff
    public class TranslationUnit
    {
        [Theory]
        [InlineData("basic")]
        [InlineData("example with spaces")]
        [InlineData("♫")]
        public void Basic(string name)
        {
            // Create a unique directory
            var dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(dir);

            try
            {
                // Create a file with the right name
                var file = new FileInfo(Path.Combine(dir, name + ".c"));
                File.WriteAllText(file.FullName, "int main() { return 0; }");

                var index = clang.createIndex(0, 0);
                var translationUnit = clang.parseTranslationUnit(index, file.FullName, new string[0], 0, new CXUnsavedFile[0], 0, 0);
                var clangFile = clang.getFile(translationUnit, file.FullName);
                var clangFileName = clang.getFileName(clangFile);
                var clangFileNameString = clang.getCString(clangFileName);

                Assert.Equal(file.FullName, clangFileNameString);
            }
            finally
            {
                Directory.Delete(dir, true);
            }
        }

        [Theory]
        [InlineData("basic")]
        [InlineData("example with spaces")]
        [InlineData("♫")]
        public void BasicWrapper(string name)
        {
            // Create a unique directory
            var dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(dir);

            try
            {
                // Create a file with the right name
                var file = new FileInfo(Path.Combine(dir, name + ".c"));
                File.WriteAllText(file.FullName, "int main() { return 0; }");

                var index = CXIndex.Create();
                var translationUnit = CXTranslationUnit.Parse(index, file.FullName, Array.Empty<string>(), Array.Empty<CXUnsavedFile>(), CXTranslationUnit_Flags.CXTranslationUnit_None);
                var clangFile = translationUnit.GetFile(file.FullName);
                var clangFileName = clangFile.Name;
                var clangFileNameString = clangFileName.CString;

                Assert.Equal(file.FullName, clangFileNameString);
            }
            finally
            {
                Directory.Delete(dir, true);
            }
        }
    }
}
