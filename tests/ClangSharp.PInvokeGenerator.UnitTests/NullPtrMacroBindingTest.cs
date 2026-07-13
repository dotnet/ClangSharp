// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

/// <summary>Provides validation that a <c>nullptr</c> macro is generated as a <c>void*</c> constant rather than the invalid type name <c>null</c>.</summary>
public sealed class NullPtrMacroBindingTest : PInvokeGeneratorTest
{
    [Test]
    public Task NullPtrMacroTest()
    {
        var inputContents = @"#define MY_NULL_HANDLE nullptr";

        var expectedOutputContents = @"namespace ClangSharp.Test
{
    public static unsafe partial class Methods
    {
        [NativeTypeName(""#define MY_NULL_HANDLE nullptr"")]
        public static readonly void* MY_NULL_HANDLE = null;
    }
}
";

        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
    }
}
