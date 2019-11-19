// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

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
    }
}
