using System;
using System.Threading.Tasks;
using Xunit;

namespace ClangSharpPInvokeGenerator.Test
{
    public abstract class PInvokeGeneratorTest : IDisposable
    {
        private TemporaryFile _inputFile = new TemporaryFile();
        private TemporaryFile _outputFile = new TemporaryFile();

        public TemporaryFile InputFile => _inputFile;

        public TemporaryFile OutputFile => _outputFile;

        public void Dispose()
        {
            if (_inputFile != null)
            {
                _inputFile.Dispose();
                _inputFile = null;
            }

            if (_outputFile != null)
            {
                _outputFile.Dispose();
                _outputFile = null;
            }
        }

        protected async Task ValidateGeneratedBindings(string inputContents, string expectedOutputContents)
        {
            await InputFile.WriteAllText(inputContents);
            await GenerateBindings();

            var actualOutputContents = await OutputFile.ReadAllText();
            Assert.Equal(expectedOutputContents, actualOutputContents);
        }

        protected async Task GenerateBindings()
        {
            await Program.Main(
                "-f", InputFile.Path,
                "-l", "ClangSharpPInvokeGenerator.Test",
                "-n", "ClangSharpPInvokeGenerator.Test",
                "-o", OutputFile.Path
            );
        }
    }
}
