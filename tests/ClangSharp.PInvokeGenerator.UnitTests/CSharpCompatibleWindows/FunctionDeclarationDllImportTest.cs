// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClangSharp.UnitTests
{
    public sealed class CSharpCompatibleWindows_FunctionDeclarationDllImportTest : FunctionDeclarationDllImportTest
    {
        public override Task BasicTest()
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

            return ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task ArrayParameterTest()
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

            return ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task FunctionPointerParameterTest()
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

            return ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task TemplateParameterTest(string nativeType, bool expectedNativeTypeAttr, string expectedManagedType, string expectedUsingStatement)
        {
            var inputContents = @$"template <typename T> struct MyTemplate;

void MyFunction(MyTemplate<{nativeType}> myStruct);";

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

            return ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents, excludedNames: new[] { "MyTemplate" });
        }

        public override Task TemplateMemberTest()
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

            return ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents, excludedNames: new[] { "MyTemplate" });
        }

        public override Task NoLibraryPathTest()
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

            return ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents, libraryPath: string.Empty);
        }

        public override Task WithLibraryPathTest()
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
            return ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents, libraryPath: string.Empty, withLibraryPaths: withLibraryPaths);
        }

        public override Task WithLibraryPathStarTest()
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
            return ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents, libraryPath: string.Empty, withLibraryPaths: withLibraryPaths);
        }

        public override Task OptionalParameterTest(string nativeType, string nativeInit, bool expectedNativeTypeNameAttr, string expectedManagedType, string expectedManagedInit)
        {
            var inputContents = $@"void MyFunction({nativeType} value = {nativeInit});";

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

            return ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task OptionalParameterUnsafeTest(string nativeType, string nativeInit, string expectedManagedType, string expectedManagedInit)
        {
            var inputContents = $@"void MyFunction({nativeType} value = {nativeInit});";

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

            return ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task WithCallConvTest()
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
            return ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents, withCallConvs: withCallConvs);
        }

        public override Task WithCallConvStarTest()
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
            return ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents, withCallConvs: withCallConvs);
        }

        public override Task WithCallConvStarOverrideTest()
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
            return ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents, withCallConvs: withCallConvs);
        }

        public override Task WithSetLastErrorTest()
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
            return ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents, withSetLastErrors: withSetLastErrors);
        }

        public override Task WithSetLastErrorStarTest()
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
            return ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents, withSetLastErrors: withSetLastErrors);
        }
    }
}
