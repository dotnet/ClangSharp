using System.Threading.Tasks;
using Xunit;

namespace ClangSharp.Test
{
    public sealed class EnumDeclarationTest : PInvokeGeneratorTest
    {
        [Fact]
        public async Task BasicTest()
        {
            var inputContents = @"enum MyEnum
{
    MyEnum_Value0,
    MyEnum_Value1,
    MyEnum_Value2,
};
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public enum MyEnum
    {
        MyEnum_Value0,
        MyEnum_Value1,
        MyEnum_Value2,
    }
}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task BasicValueTest()
        {
            var inputContents = @"enum MyEnum
{
    MyEnum_Value1 = 1,
    MyEnum_Value2,
    MyEnum_Value3,
};
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public enum MyEnum
    {
        MyEnum_Value1 = 1,
        MyEnum_Value2,
        MyEnum_Value3,
    }
}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task BasicTypedTest()
        {
            var inputContents = @"enum MyEnum : unsigned char
{
    MyEnum_Value0,
    MyEnum_Value1,
    MyEnum_Value2,
};
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public enum MyEnum : byte
    {
        MyEnum_Value0,
        MyEnum_Value1,
        MyEnum_Value2,
    }
}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }
    }
}
