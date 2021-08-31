// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Xunit;

namespace ClangSharp.UnitTests
{
    public abstract class CXXMethodDeclarationTest : PInvokeGeneratorTest
    {
        [Fact]
        public abstract Task ConstructorTest();

        [Fact]
        public abstract Task ConstructorWithInitializeTest();

        [Fact]
        public abstract Task ConstructorImportTest();

        [Fact]
        public abstract Task CopyAndMoveConstructor();

        [Fact]
        public abstract Task ConversionTest();

        [Fact]
        public abstract Task DestructorTest();

        [Fact]
        public abstract Task InstanceTest();

        [Fact]
        public abstract Task MemberCallTest();

        [Fact]
        public abstract Task MemberTest();

        [Fact]
        public abstract Task NewKeywordTest();

        [Fact]
        public abstract Task NewKeywordVirtualTest();

        [Fact]
        public abstract Task NewKeywordVirtualWithExplicitVtblTest();

        [Fact]
        public abstract Task OperatorTest();

        [Fact]
        public abstract Task OperatorCallTest();

        [Fact]
        public abstract Task StaticTest();

        [Fact]
        public abstract Task ThisTest();

        [Fact]
        public abstract Task UnsafeDoesNotImpactDllImportTest();

        [Fact]
        public abstract Task VirtualTest();

        [Fact]
        public abstract Task VirtualWithVtblIndexAttributeTest();
    }
}
