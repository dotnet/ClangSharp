// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Threading.Tasks;

namespace ClangSharp.UnitTests
{
    public sealed class CSharpLatestUnix_FunctionPointerDeclarationTest : FunctionPointerDeclarationTest
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

            return ValidateGeneratedCSharpLatestUnixBindingsAsync(inputContents, expectedOutputContents);
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

            return ValidateGeneratedCSharpLatestUnixBindingsAsync(inputContents, expectedOutputContents);
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

            return ValidateGeneratedCSharpLatestUnixBindingsAsync(inputContents, expectedOutputContents);
        }
    }
}
