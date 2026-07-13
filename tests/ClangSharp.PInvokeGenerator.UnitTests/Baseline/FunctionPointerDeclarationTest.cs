// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests.Baseline;

// FunctionPointerDeclarationTest migrated to the baseline model. One fixture, parameterized over all 16
// variants; each case declares its C/C++ input exactly once and the expected output lives in a checked-in
// baseline file.
[TestFixtureSource(nameof(Variants))]
public sealed class FunctionPointerDeclarationTest : BaselineTest
{
    public FunctionPointerDeclarationTest(BaselineVariant variant) : base(variant)
    {
    }

    protected override string Area => "FunctionPointerDeclaration";

    [Test]
    public Task BasicTest()
        => ValidateAsync(nameof(BasicTest), @"typedef void (*Callback)();

struct MyStruct {
    Callback _callback;
};
");

    [Test]
    public Task CallconvTest()
        => ValidateAsync(nameof(CallconvTest), @"typedef void (*Callback)() __attribute__((stdcall));

struct MyStruct {
    Callback _callback;
};
");

    [Test]
    public Task PointerlessTypedefTest()
        => ValidateAsync(nameof(PointerlessTypedefTest), @"typedef void (Callback)();

struct MyStruct {
    Callback* _callback;
};
");
}
