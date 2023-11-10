// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

[Platform("win")]
public sealed class CSharpCompatibleWindows_FunctionPointerDeclarationTest : FunctionPointerDeclarationTest
{
    protected override Task BasicTestImpl()
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

    protected override Task CallconvTestImpl()
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

    protected override Task PointerlessTypedefTestImpl()
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
