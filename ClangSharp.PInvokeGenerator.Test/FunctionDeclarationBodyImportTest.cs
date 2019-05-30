using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ClangSharp.Test
{
    public sealed class FunctionDeclarationBodyImportTest : PInvokeGeneratorTest
    {
        [Fact]
        public async Task BasicTest()
        {
            var inputContents = @"void MyFunction()
{
}
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public static partial class Methods
    {
        private const string libraryPath = ""ClangSharpPInvokeGenerator"";

        public static void MyFunction()
        {
        }
    }
}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task CallFunctionTest()
        {
            var inputContents = @"void MyCalledFunction()
{
}

void MyFunction()
{
    MyCalledFunction();
}
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public static partial class Methods
    {
        private const string libraryPath = ""ClangSharpPInvokeGenerator"";

        public static void MyCalledFunction()
        {
        }

        public static void MyFunction()
        {
            MyCalledFunction();
        }
    }
}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task ReturnIntegerTest()
        {
            var inputContents = @"int MyFunction()
{
    return -1;
}
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public static partial class Methods
    {
        private const string libraryPath = ""ClangSharpPInvokeGenerator"";

        public static int MyFunction()
        {
            return -1;
        }
    }
}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }
    }
}
