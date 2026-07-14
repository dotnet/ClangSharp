// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Threading.Tasks;
using ClangSharp.UnitTests.Baseline;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

public sealed class GlobalStructInitializerTest : StandaloneBaselineTest
{
    protected override string Area => "GlobalStructInitializer";

    // Regression test for https://github.com/dotnet/clangsharp/issues/503
    // A non-const global variable initialized with a struct initializer list was
    // missing its terminating semicolon, which corrupted the rest of the file.
    [Test]
    public Task NonConstGlobalTest()
    {
        var inputContents = @"typedef struct Point { int x; int y; } Point;

Point MyGlobalPoint = { .x = 10, .y = 20 };
";

        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(inputContents);
    }
}
