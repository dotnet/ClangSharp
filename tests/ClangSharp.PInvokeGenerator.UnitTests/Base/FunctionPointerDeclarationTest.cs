// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Threading.Tasks;
using Xunit;

namespace ClangSharp.UnitTests
{
    public abstract class FunctionPointerDeclarationTest : PInvokeGeneratorTest
    {
        [Fact]
        public abstract Task BasicTest();

        [Fact]
        public abstract Task CallconvTest();

        [Fact]
        public abstract Task PointerlessTypedefTest();
    }
}
