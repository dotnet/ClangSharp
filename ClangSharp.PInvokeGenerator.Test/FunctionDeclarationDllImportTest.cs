using System.Threading.Tasks;
using Xunit;

namespace ClangSharp.Test
{
    public sealed class FunctionDeclarationDllImportTest : PInvokeGeneratorTest
    {
        [Fact]
        public async Task BasicTest()
        {
            var inputContents = @"void MyFunction();";

            var expectedOutputContents = @"using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public static partial class Methods
    {
        private const string libraryPath = ""ClangSharpPInvokeGenerator"";

        [DllImport(libraryPath, EntryPoint = ""MyFunction"", CallingConvention = CallingConvention.Cdecl)]
        public static extern void MyFunction();
    }
}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }
    }
}
