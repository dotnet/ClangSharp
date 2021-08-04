// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Threading.Tasks;

namespace ClangSharp.UnitTests
{
    public sealed class CSharpLatestWindows_FunctionPointerDeclarationTest : FunctionPointerDeclarationTest
    {
        public override Task BasicTest()
        {
            var inputContents = @"typedef void (*Callback)();

struct MyStruct {
    Callback _callback;
};
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public unsafe partial struct MyStruct
    {
        [NativeTypeName(""Callback"")]
        public delegate* unmanaged[Cdecl]<void> _callback;
    }
}
";

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task CallconvTest()
        {
            var inputContents = @"typedef void (*Callback)() __attribute__((stdcall));

struct MyStruct {
    Callback _callback;
};
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public unsafe partial struct MyStruct
    {
        [NativeTypeName(""Callback"")]
        public delegate* unmanaged[Stdcall]<void> _callback;
    }
}
";

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task PointerlessTypedefTest()
        {
            var inputContents = @"typedef void (Callback)();

struct MyStruct {
    Callback* _callback;
};
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public unsafe partial struct MyStruct
    {
        [NativeTypeName(""Callback *"")]
        public delegate* unmanaged[Cdecl]<void> _callback;
    }
}
";

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }
    }
}
