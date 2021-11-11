// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

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
