// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using ClangSharp.UnitTests.Baseline;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

public sealed class UnsafeFixedSizeBufferTest : StandaloneBaselineTest
{
    protected override string Area => "UnsafeFixedSizeBuffer";

    [Test]
    public Task RemappedFixedSizeBufferLowersToPointerAndMarksClassUnsafe()
    {
        var inputContents = @"typedef int MyBuffer;

extern ""C"" void MyFunction(MyBuffer value);
extern ""C"" MyBuffer MyOtherFunction();";

        var remappedNames = new Dictionary<string, string> {
            ["MyBuffer"] = "sbyte[8]"
        };

        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(inputContents, remappedNames: remappedNames);
    }
}
