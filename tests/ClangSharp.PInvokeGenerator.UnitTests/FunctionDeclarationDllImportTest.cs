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
        [DllImport(""ClangSharpPInvokeGenerator"", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void MyFunction();
    }
}
";

            await ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task ArrayParameterTest()
        {
            var inputContents = @"void MyFunction(const float color[4]);";

            var expectedOutputContents = @"using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public static unsafe partial class Methods
    {
        [DllImport(""ClangSharpPInvokeGenerator"", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void MyFunction([NativeTypeName(""const float [4]"")] float* color);
    }
}
";

            await ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task FunctionPointerParameterTest()
        {
            var inputContents = @"void MyFunction(void (*callback)());";

            var expectedOutputContents = @"using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public static partial class Methods
    {
        [DllImport(""ClangSharpPInvokeGenerator"", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void MyFunction([NativeTypeName(""void (*)()"")] IntPtr callback);
    }
}
";

            await ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents);
        }

        [Theory]
        [InlineData("MyStruct<int>", @"MyStruct<int>", "")]
        [InlineData("MyStruct<bool>", @"[NativeTypeName(""MyStruct<bool>"")] MyStruct<byte>", "")]
        [InlineData("MyStruct<float*>", @"[NativeTypeName(""MyStruct<float *>"")] MyStruct<UIntPtr>", "using System;\n")]
        public async Task TemplateParameterTest(string nativeParameter, string expectedManagedParameter, string expectedUsingStatement)
        {
            var inputContents = @$"template <typename T> struct MyStruct;

void MyFunction({nativeParameter} myStruct);";

            var expectedOutputContents = $@"{expectedUsingStatement}using System.Runtime.InteropServices;

namespace ClangSharp.Test
{{
    public static partial class Methods
    {{
        [DllImport(""ClangSharpPInvokeGenerator"", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void MyFunction({expectedManagedParameter} myStruct);
    }}
}}
";

            await ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, excludedNames: new[] { "MyStruct" });
        }


        [Fact]
        public async Task NoLibraryPathTest()
        {
            var inputContents = @"void MyFunction();";

            var expectedOutputContents = @"using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public static partial class Methods
    {
        [DllImport("""", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void MyFunction();
    }
}
";

            await ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, libraryPath: string.Empty);
        }

        [Fact]
        public async Task WithLibraryPathTest()
        {
            var inputContents = @"void MyFunction();";

            var expectedOutputContents = @"using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public static partial class Methods
    {
        [DllImport(""ClangSharpPInvokeGenerator"", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void MyFunction();
    }
}
";

            var withLibraryPaths = new Dictionary<string, string>
            {
                ["MyFunction"] = "ClangSharpPInvokeGenerator"
            };
            await ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, libraryPath: string.Empty, withLibraryPaths: withLibraryPaths);
        }

        [Fact]
        public async Task WithLibraryPathStarTest()
        {
            var inputContents = @"void MyFunction();";

            var expectedOutputContents = @"using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public static partial class Methods
    {
        [DllImport(""ClangSharpPInvokeGenerator"", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void MyFunction();
    }
}
";

            var withLibraryPaths = new Dictionary<string, string>
            {
                ["*"] = "ClangSharpPInvokeGenerator"
            };
            await ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, libraryPath: string.Empty, withLibraryPaths: withLibraryPaths);
        }

        [Theory]
        [InlineData("unsigned char value = 0", @"[NativeTypeName(""unsigned char"")] byte value = 0")]
        [InlineData("double value = 1.0", @"double value = 1.0")]
        [InlineData("short value = 2", @"short value = 2")]
        [InlineData("int value = 3", @"int value = 3")]
        [InlineData("long long value = 4", @"[NativeTypeName(""long long"")] long value = 4")]
        [InlineData("signed char value = 5", @"[NativeTypeName(""signed char"")] sbyte value = 5")]
        [InlineData("float value = 6.0f", @"float value = 6.0f")]
        [InlineData("unsigned short value = 7", @"[NativeTypeName(""unsigned short"")] ushort value = 7")]
        [InlineData("unsigned int value = 8", @"[NativeTypeName(""unsigned int"")] uint value = 8")]
        [InlineData("unsigned long long value = 9", @"[NativeTypeName(""unsigned long long"")] ulong value = 9")]
        [InlineData("unsigned short value = 'A'", @"[NativeTypeName(""unsigned short"")] ushort value = (byte)('A')")]
        public async Task OptionalParameterTest(string nativeParameters, string expectedManagedParameters)
        {
            var inputContents = $@"void MyFunction({nativeParameters});";

            var expectedOutputContents = $@"using System.Runtime.InteropServices;

namespace ClangSharp.Test
{{
    public static partial class Methods
    {{
        [DllImport(""ClangSharpPInvokeGenerator"", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void MyFunction({expectedManagedParameters});
    }}
}}
";

            await ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents);
        }

        [Theory]
        [InlineData("void* value = nullptr", @"[NativeTypeName(""void *"")] void* value = null")]
        [InlineData("void* value = 0", @"[NativeTypeName(""void *"")] void* value = null")]
        public async Task OptionalParameterUnsafeTest(string nativeParameters, string expectedManagedParameters)
        {
            var inputContents = $@"void MyFunction({nativeParameters});";

            var expectedOutputContents = $@"using System.Runtime.InteropServices;

namespace ClangSharp.Test
{{
    public static unsafe partial class Methods
    {{
        [DllImport(""ClangSharpPInvokeGenerator"", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void MyFunction({expectedManagedParameters});
    }}
}}
";

            await ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents);
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
        [DllImport(""ClangSharpPInvokeGenerator"", ExactSpelling = true)]
        public static extern void MyFunction1(int value);

        [DllImport(""ClangSharpPInvokeGenerator"", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void MyFunction2(int value);
    }
}
";

            var withCallConvs = new Dictionary<string, string> {
                ["MyFunction1"] = "Winapi"
            };
            await ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, withCallConvs: withCallConvs);
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
        [DllImport(""ClangSharpPInvokeGenerator"", ExactSpelling = true)]
        public static extern void MyFunction1(int value);

        [DllImport(""ClangSharpPInvokeGenerator"", ExactSpelling = true)]
        public static extern void MyFunction2(int value);
    }
}
";

            var withCallConvs = new Dictionary<string, string>
            {
                ["*"] = "Winapi"
            };
            await ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, withCallConvs: withCallConvs);
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
        [DllImport(""ClangSharpPInvokeGenerator"", ExactSpelling = true)]
        public static extern void MyFunction1(int value);

        [DllImport(""ClangSharpPInvokeGenerator"", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        public static extern void MyFunction2(int value);
    }
}
";

            var withCallConvs = new Dictionary<string, string>
            {
                ["*"] = "Winapi",
                ["MyFunction2"] = "StdCall"
            };
            await ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, withCallConvs: withCallConvs);
        }

        [Fact]
        public async Task WithSetLastErrorTest()
        {
            var inputContents = @"void MyFunction1(int value); void MyFunction2(int value);";

            var expectedOutputContents = @"using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public static partial class Methods
    {
        [DllImport(""ClangSharpPInvokeGenerator"", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, SetLastError = true)]
        public static extern void MyFunction1(int value);

        [DllImport(""ClangSharpPInvokeGenerator"", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void MyFunction2(int value);
    }
}
";

            var withSetLastErrors = new string[]
            {
                "MyFunction1"
            };
            await ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, withSetLastErrors: withSetLastErrors);
        }

        [Fact]
        public async Task WithSetLastErrorStarTest()
        {
            var inputContents = @"void MyFunction1(int value); void MyFunction2(int value);";

            var expectedOutputContents = @"using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public static partial class Methods
    {
        [DllImport(""ClangSharpPInvokeGenerator"", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, SetLastError = true)]
        public static extern void MyFunction1(int value);

        [DllImport(""ClangSharpPInvokeGenerator"", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, SetLastError = true)]
        public static extern void MyFunction2(int value);
    }
}
";

            var withSetLastErrors = new string[]
            {
                "*"
            };
            await ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, withSetLastErrors: withSetLastErrors);
        }
    }
}
