// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ClangSharp.UnitTests
{
    public sealed class CSharpPreviewUnix_FunctionDeclarationDllImportTest : FunctionDeclarationDllImportTest
    {
        protected override Task BasicTestImpl()
        {
            var inputContents = @"extern ""C"" void MyFunction();";

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

            return ValidateGeneratedCSharpPreviewUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        protected override Task ArrayParameterTestImpl()
        {
            var inputContents = @"extern ""C"" void MyFunction(const float color[4]);";

            var expectedOutputContents = @"using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public static unsafe partial class Methods
    {
        [DllImport(""ClangSharpPInvokeGenerator"", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void MyFunction([NativeTypeName(""const float[4]"")] float* color);
    }
}
";

            return ValidateGeneratedCSharpPreviewUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        protected override Task FunctionPointerParameterTestImpl()
        {
            var inputContents = @"extern ""C"" void MyFunction(void (*callback)());";

            var expectedOutputContents = @"using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public static unsafe partial class Methods
    {
        [DllImport(""ClangSharpPInvokeGenerator"", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void MyFunction([NativeTypeName(""void (*)()"")] delegate* unmanaged[Cdecl]<void> callback);
    }
}
";

            return ValidateGeneratedCSharpPreviewUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        protected override Task NamespaceTestImpl()
        {
            var inputContents = @"namespace MyNamespace
{
    void MyFunction();
}";

            var entryPoint = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "__ZN11MyNamespace10MyFunctionEv" : "_ZN11MyNamespace10MyFunctionEv";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                entryPoint = "?MyFunction@MyNamespace@@YAXXZ";
            }

            var expectedOutputContents = $@"using System.Runtime.InteropServices;

namespace ClangSharp.Test
{{
    public static partial class Methods
    {{
        [DllImport(""ClangSharpPInvokeGenerator"", CallingConvention = CallingConvention.Cdecl, EntryPoint = ""{entryPoint}"", ExactSpelling = true)]
        public static extern void MyFunction();
    }}
}}
";

            return ValidateGeneratedCSharpPreviewUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        protected override Task TemplateParameterTestImpl(string nativeType, bool expectedNativeTypeAttr, string expectedManagedType, string expectedUsingStatement)
        {
            var inputContents = @$"template <typename T> struct MyTemplate;

extern ""C"" void MyFunction(MyTemplate<{nativeType}> myStruct);";

            var expectedOutputContents = $@"{expectedUsingStatement}using System.Runtime.InteropServices;

namespace ClangSharp.Test
{{
    public static partial class Methods
    {{
        [DllImport(""ClangSharpPInvokeGenerator"", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void MyFunction({(expectedNativeTypeAttr ? $@"[NativeTypeName(""MyTemplate<{nativeType}>"")] " : "")}MyTemplate<{expectedManagedType}> myStruct);
    }}
}}
";

            return ValidateGeneratedCSharpPreviewUnixBindingsAsync(inputContents, expectedOutputContents, excludedNames: new[] { "MyTemplate" });
        }

        protected override Task TemplateMemberTestImpl()
        {
            var inputContents = @$"template <typename T> struct MyTemplate
{{
}};

struct MyStruct
{{
    MyTemplate<float*> a;
}};
";

            var expectedOutputContents = $@"using System;

namespace ClangSharp.Test
{{
    public partial struct MyStruct
    {{
        [NativeTypeName(""MyTemplate<float *>"")]
        public MyTemplate<IntPtr> a;
    }}
}}
";

            return ValidateGeneratedCSharpPreviewUnixBindingsAsync(inputContents, expectedOutputContents, excludedNames: new[] { "MyTemplate" });
        }

        protected override Task NoLibraryPathTestImpl()
        {
            var inputContents = @"extern ""C"" void MyFunction();";

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

            return ValidateGeneratedCSharpPreviewUnixBindingsAsync(inputContents, expectedOutputContents, libraryPath: string.Empty);
        }

        protected override Task WithLibraryPathTestImpl()
        {
            var inputContents = @"extern ""C"" void MyFunction();";

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
            return ValidateGeneratedCSharpPreviewUnixBindingsAsync(inputContents, expectedOutputContents, libraryPath: string.Empty, withLibraryPaths: withLibraryPaths);
        }

        protected override Task WithLibraryPathStarTestImpl()
        {
            var inputContents = @"extern ""C"" void MyFunction();";

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
            return ValidateGeneratedCSharpPreviewUnixBindingsAsync(inputContents, expectedOutputContents, libraryPath: string.Empty, withLibraryPaths: withLibraryPaths);
        }

        protected override Task OptionalParameterTestImpl(string nativeType, string nativeInit, bool expectedNativeTypeNameAttr, string expectedManagedType, string expectedManagedInit)
        {
            var inputContents = $@"extern ""C"" void MyFunction({nativeType} value = {nativeInit});";

            var expectedOutputContents = $@"using System.Runtime.InteropServices;

namespace ClangSharp.Test
{{
    public static partial class Methods
    {{
        [DllImport(""ClangSharpPInvokeGenerator"", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void MyFunction({(expectedNativeTypeNameAttr ? $@"[NativeTypeName(""{nativeType}"")] " : "")}{expectedManagedType} value = {expectedManagedInit});
    }}
}}
";

            return ValidateGeneratedCSharpPreviewUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        protected override Task OptionalParameterUnsafeTestImpl(string nativeType, string nativeInit, string expectedManagedType, string expectedManagedInit)
        {
            var inputContents = $@"extern ""C"" void MyFunction({nativeType} value = {nativeInit});";

            var expectedOutputContents = $@"using System.Runtime.InteropServices;

namespace ClangSharp.Test
{{
    public static unsafe partial class Methods
    {{
        [DllImport(""ClangSharpPInvokeGenerator"", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void MyFunction({expectedManagedType} value = {expectedManagedInit});
    }}
}}
";

            return ValidateGeneratedCSharpPreviewUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        protected override Task WithCallConvTestImpl()
        {
            var inputContents = @"extern ""C"" void MyFunction1(int value); extern ""C"" void MyFunction2(int value);";

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
            return ValidateGeneratedCSharpPreviewUnixBindingsAsync(inputContents, expectedOutputContents, withCallConvs: withCallConvs);
        }

        protected override Task WithCallConvStarTestImpl()
        {
            var inputContents = @"extern ""C"" void MyFunction1(int value); extern ""C"" void MyFunction2(int value);";

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
            return ValidateGeneratedCSharpPreviewUnixBindingsAsync(inputContents, expectedOutputContents, withCallConvs: withCallConvs);
        }

        protected override Task WithCallConvStarOverrideTestImpl()
        {
            var inputContents = @"extern ""C"" void MyFunction1(int value); extern ""C"" void MyFunction2(int value);";

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
            return ValidateGeneratedCSharpPreviewUnixBindingsAsync(inputContents, expectedOutputContents, withCallConvs: withCallConvs);
        }

        protected override Task WithSetLastErrorTestImpl()
        {
            var inputContents = @"extern ""C"" void MyFunction1(int value); extern ""C"" void MyFunction2(int value);";

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
            return ValidateGeneratedCSharpPreviewUnixBindingsAsync(inputContents, expectedOutputContents, withSetLastErrors: withSetLastErrors);
        }

        protected override Task WithSetLastErrorStarTestImpl()
        {
            var inputContents = @"extern ""C"" void MyFunction1(int value); extern ""C"" void MyFunction2(int value);";

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
            return ValidateGeneratedCSharpPreviewUnixBindingsAsync(inputContents, expectedOutputContents, withSetLastErrors: withSetLastErrors);
        }

        protected override Task SourceLocationTestImpl()
        {
            const string InputContents = @"extern ""C"" void MyFunction(float value);";

            const string ExpectedOutputContents = @"using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public static partial class Methods
    {
        [DllImport(""ClangSharpPInvokeGenerator"", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SourceLocation(""ClangUnsavedFile.h"", 1, 17)]
        public static extern void MyFunction([SourceLocation(""ClangUnsavedFile.h"", 1, 34)] float value);
    }
}
";

            return ValidateGeneratedCSharpPreviewUnixBindingsAsync(InputContents, ExpectedOutputContents, PInvokeGeneratorConfigurationOptions.GenerateSourceLocationAttribute);
        }

        protected override Task VarargsTestImpl() => Task.CompletedTask;
    }
}
