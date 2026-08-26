// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Threading.Tasks;
using ClangSharp.UnitTests.Baseline;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

public sealed class SizeOfMacroTestTest : StandaloneBaselineTest
{
    protected override string Area => "SizeOfMacroTestTest";

    [Test]
    public Task RuntimeSizeofMacroTest()
    {
        var inputContents = """
            typedef struct XrEventDataBuffer {
                const void* next;
            } XrEventDataBuffer;

            #define XR_MAX_EVENT_DATA_SIZE sizeof(XrEventDataBuffer)
            """;

        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(inputContents);
    }
}
