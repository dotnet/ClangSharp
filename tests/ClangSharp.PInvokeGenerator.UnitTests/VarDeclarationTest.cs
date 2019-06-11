using System.Threading.Tasks;
using Xunit;

namespace ClangSharp.UnitTests
{
    public sealed class VarDeclarationTest : PInvokeGeneratorTest
    {
        [Theory]
        [InlineData("double", "double")]
        [InlineData("short", "short")]
        [InlineData("int", "int")]
        [InlineData("float", "float")]
        public async Task BasicTest(string nativeType, string expectedManagedType)
        {
            var inputContents = $@"{nativeType} MyVariable;";

            var expectedOutputContents = $@"namespace ClangSharp.Test
{{
    public static partial class Methods
    {{
        private const string libraryPath = ""ClangSharpPInvokeGenerator"";

        // public static extern {expectedManagedType} MyVariable;
    }}
}}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Theory]
        [InlineData("unsigned char", "byte")]
        [InlineData("long long", "long")]
        [InlineData("signed char", "sbyte")]
        [InlineData("unsigned short", "ushort")]
        [InlineData("unsigned int", "uint")]
        [InlineData("unsigned long long", "ulong")]
        public async Task BasicWithNativeTypeNameTest(string nativeType, string expectedManagedType)
        {
            var inputContents = $@"{nativeType} MyVariable;";

            var expectedOutputContents = $@"namespace ClangSharp.Test
{{
    public static partial class Methods
    {{
        private const string libraryPath = ""ClangSharpPInvokeGenerator"";

        // [NativeTypeName(""{nativeType}"")]
        // public static extern {expectedManagedType} MyVariable;
    }}
}}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }
    }
}
