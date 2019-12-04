// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ClangSharp.UnitTests
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
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = ""MyFunction"", ExactSpelling = true)]
        public static extern void MyFunction();
    }
}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task ArrayParameterTest()
        {
            var inputContents = @"void MyFunction(const float color[4]);";

            var expectedOutputContents = @"using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public static partial class Methods
    {
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = ""MyFunction"", ExactSpelling = true)]
        public static extern void MyFunction([NativeTypeName(""const float [4]"")] float* color);
    }
}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Theory]
        [InlineData("int value = 0", "int value = 0")]
        [InlineData("unsigned short value = '1'", @"[NativeTypeName(""unsigned short"")] ushort value = '1'")]
        public async Task OptionalParameterTest(string nativeParameters, string expectedManagedParameters)
        {
            var inputContents = $@"void MyFunction({nativeParameters});";

            var expectedOutputContents = $@"using System.Runtime.InteropServices;

namespace ClangSharp.Test
{{
    public static partial class Methods
    {{
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = ""MyFunction"", ExactSpelling = true)]
        public static extern void MyFunction({expectedManagedParameters});
    }}
}}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task WithCallConvTest()
        {
            var inputContents = @"void MyFunction1(int value); void MyFunction2(int value);";

            var expectedOutputContents = @"using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public static partial class Methods
    {
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Winapi, EntryPoint = ""MyFunction1"", ExactSpelling = true)]
        public static extern void MyFunction1(int value);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = ""MyFunction2"", ExactSpelling = true)]
        public static extern void MyFunction2(int value);
    }
}
";

            var withCallConvs = new Dictionary<string, string> {
                ["MyFunction1"] = "Winapi"
            };
            await ValidateGeneratedBindings(inputContents, expectedOutputContents, withCallConvs: withCallConvs);
        }

        [Fact]
        public async Task WithCallConvStarTest()
        {
            var inputContents = @"void MyFunction1(int value); void MyFunction2(int value);";

            var expectedOutputContents = @"using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public static partial class Methods
    {
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Winapi, EntryPoint = ""MyFunction1"", ExactSpelling = true)]
        public static extern void MyFunction1(int value);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Winapi, EntryPoint = ""MyFunction2"", ExactSpelling = true)]
        public static extern void MyFunction2(int value);
    }
}
";

            var withCallConvs = new Dictionary<string, string>
            {
                ["*"] = "Winapi"
            };
            await ValidateGeneratedBindings(inputContents, expectedOutputContents, withCallConvs: withCallConvs);
        }

        [Fact]
        public async Task WithCallConvStarOverrideTest()
        {
            var inputContents = @"void MyFunction1(int value); void MyFunction2(int value);";

            var expectedOutputContents = @"using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public static partial class Methods
    {
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Winapi, EntryPoint = ""MyFunction1"", ExactSpelling = true)]
        public static extern void MyFunction1(int value);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.StdCall, EntryPoint = ""MyFunction2"", ExactSpelling = true)]
        public static extern void MyFunction2(int value);
    }
}
";

            var withCallConvs = new Dictionary<string, string>
            {
                ["*"] = "Winapi",
                ["MyFunction2"] = "StdCall"
            };
            await ValidateGeneratedBindings(inputContents, expectedOutputContents, withCallConvs: withCallConvs);
        }
    }
}
