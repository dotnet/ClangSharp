// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

[TestFixtureSource(nameof(FixtureArgs))]
public sealed class FunctionPointerDeclarationTest(PInvokeGeneratorOutputMode outputMode, PInvokeGeneratorConfigurationOptions outputVersion)
    : PInvokeGeneratorTest(outputMode, outputVersion)
{
    [Test]
    public Task BasicTest()
    {
        var inputContents = @"typedef void (*Callback)();

struct MyStruct {
    Callback _callback;
};
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task CallconvTest()
    {
        var inputContents = @"typedef void (*Callback)() __attribute__((stdcall));

struct MyStruct {
    Callback _callback;
};
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }

    [Test]
    public Task PointerlessTypedefTest()
    {
        var inputContents = @"typedef void (Callback)();

struct MyStruct {
    Callback* _callback;
};
";

        return ValidateGeneratedBindingsAsync(inputContents);
    }
}
