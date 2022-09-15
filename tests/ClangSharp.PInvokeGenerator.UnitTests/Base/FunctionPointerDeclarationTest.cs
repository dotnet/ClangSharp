// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

public abstract class FunctionPointerDeclarationTest : PInvokeGeneratorTest
{
    [Test]
    public Task BasicTest() => BasicTestImpl();

    [Test]
    public Task CallconvTest() => CallconvTestImpl();

    [Test]
    public Task PointerlessTypedefTest() => PointerlessTypedefTestImpl();

    protected abstract Task BasicTestImpl();

    protected abstract Task CallconvTestImpl();

    protected abstract Task PointerlessTypedefTestImpl();
}
