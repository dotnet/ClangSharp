// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Threading.Tasks;
using ClangSharp.UnitTests.Baseline;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

/// <summary>Provides validation that a <c>nullptr</c> macro is generated as a <c>void*</c> constant rather than the invalid type name <c>null</c>.</summary>
public sealed class NullPtrMacroBindingTest : StandaloneBaselineTest
{
    protected override string Area => "NullPtrMacroBinding";

    [Test]
    public Task NullPtrMacroTest()
    {
        var inputContents = @"#define MY_NULL_HANDLE nullptr";

        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(inputContents);
    }
}
