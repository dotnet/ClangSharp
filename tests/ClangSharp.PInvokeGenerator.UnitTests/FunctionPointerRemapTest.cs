// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

/// <summary>
/// Regression test for https://github.com/dotnet/ClangSharp/issues/462.
/// In compatible mode a function-pointer typedef is emitted as a managed delegate, so a field that
/// is a pointer to that typedef must be emitted as <c>IntPtr</c> (a pointer to a managed type is
/// invalid and produces CS8500). Remapping the typedef must not change that: only the delegate name
/// is renamed while the field stays <c>IntPtr</c> and the containing struct stays non-<c>unsafe</c>.
/// </summary>
public sealed class FunctionPointerRemapTest : PInvokeGeneratorTest
{
    private const string InputContents = @"typedef void (ApiPFoo)(int);
struct ApiCallbacks { ApiPFoo* Foo; };
";

    private static readonly Dictionary<string, string> RemappedNames = new()
    {
        ["ApiCallbacks"] = "Callbacks",
        ["ApiPFoo"] = "PFoo"
    };

    private const string ExpectedOutputContents = @"using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void PFoo(int param0);

    public partial struct Callbacks
    {
        [NativeTypeName(""ApiPFoo *"")]
        public IntPtr Foo;
    }
}
";

    [Test]
    public Task RemappedFnptrTypedefFieldStaysIntPtrWindows()
    {
        return ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(InputContents, ExpectedOutputContents, remappedNames: RemappedNames, commandLineArgs: DefaultCClangCommandLineArgs, language: "c", languageStandard: DefaultCStandard);
    }

    [Test]
    public Task RemappedFnptrTypedefFieldStaysIntPtrUnix()
    {
        return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(InputContents, ExpectedOutputContents, remappedNames: RemappedNames, commandLineArgs: DefaultCClangCommandLineArgs, language: "c", languageStandard: DefaultCStandard);
    }
}
