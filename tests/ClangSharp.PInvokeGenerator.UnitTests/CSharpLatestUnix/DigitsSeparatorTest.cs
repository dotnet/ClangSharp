// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests.CSharpLatestUnix;

[Platform("unix")]
public sealed class DigitsSeparatorTest : UnitTests.DigitsSeparatorTest
{
    protected override Task StaticConstExprTestImpl(string type, string nativeValue, string expectedValue)
    {
        var inputContents = $@"class MyClass
{{
    private:

      static constexpr {type} x = {nativeValue};
}};
";

        var expectedOutputContents = $@"namespace ClangSharp.Test
{{
    public partial struct MyClass
    {{
        [NativeTypeName(""const {type}"")]
        private const {type} x = {expectedValue};
    }}
}}
";

        return ValidateGeneratedCSharpLatestUnixBindingsAsync(
            inputContents,
            expectedOutputContents);
    }
}
