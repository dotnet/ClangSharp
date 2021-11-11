// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Threading.Tasks;

namespace ClangSharp.UnitTests
{
    public sealed class CSharpCompatibleWindows_FunctionPointerDeclarationTest : FunctionPointerDeclarationTest
    {
        public override Task BasicTest()
        {
            var inputContents = @"typedef void (*Callback)();

struct MyStruct {
    Callback _callback;
};
";

            var expectedOutputContents = @"using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void Callback();

    public partial struct MyStruct
    {
        [NativeTypeName(""Callback"")]
        public IntPtr _callback;
    }
}
";

            return ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task CallconvTest()
        {
            var inputContents = @"typedef void (*Callback)() __attribute__((stdcall));

struct MyStruct {
    Callback _callback;
};
";

            var expectedOutputContents = @"using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void Callback();

    public partial struct MyStruct
    {
        [NativeTypeName(""Callback"")]
        public IntPtr _callback;
    }
}
";

            return ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task PointerlessTypedefTest()
        {
            var inputContents = @"typedef void (Callback)();

struct MyStruct {
    Callback* _callback;
};
";

            var expectedOutputContents = @"using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void Callback();

    public partial struct MyStruct
    {
        [NativeTypeName(""Callback *"")]
        public IntPtr _callback;
    }
}
";

            return ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
        }
    }
}
