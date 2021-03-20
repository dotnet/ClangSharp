// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Threading.Tasks;

namespace ClangSharp.UnitTests
{
    public sealed class CSharpCompatibleUnix_FunctionPointerDeclarationTest : FunctionPointerDeclarationTest
    {
        public override Task BasicTest()
        {
            var inputContents = @"typedef void (*Callback)();";

            var expectedOutputContents = @"using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void Callback();
}
";

            return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
        }
    }
}
