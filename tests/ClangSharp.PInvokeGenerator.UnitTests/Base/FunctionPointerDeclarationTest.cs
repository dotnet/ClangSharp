// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Threading.Tasks;
using Xunit;

namespace ClangSharp.UnitTests
{
    public abstract class FunctionPointerDeclarationTest : PInvokeGeneratorTest
    {
        [Fact]
        public abstract Task BasicTest();
    }
}
