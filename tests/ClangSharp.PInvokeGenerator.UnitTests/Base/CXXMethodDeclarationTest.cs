// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests
{
    public abstract class CXXMethodDeclarationTest : PInvokeGeneratorTest
    {
        [Test]
        public Task ConstructorTest() => ConstructorTestImpl();

        [Test]
        public Task ConstructorWithInitializeTest() => ConstructorWithInitializeTestImpl();

        [Test]
        public Task ConversionTest() => ConversionTestImpl();

        [Test]
        public Task DestructorTest() => DestructorTestImpl();

        [Test]
        public Task InstanceTest() => InstanceTestImpl();

        [Test]
        public Task MemberCallTest() => MemberCallTestImpl();

        [Test]
        public Task MemberTest() => MemberTestImpl();

        [Test]
        public Task NewKeywordTest() => NewKeywordTestImpl();

        [Test]
        public Task NewKeywordVirtualTest() => NewKeywordVirtualTestImpl();

        [Test]
        public Task NewKeywordVirtualWithExplicitVtblTest() => NewKeywordVirtualWithExplicitVtblTestImpl();

        [Test]
        public Task OperatorTest() => OperatorTestImpl();

        [Test]
        public Task OperatorCallTest() => OperatorCallTestImpl();

        [Test]
        public Task StaticTest() => StaticTestImpl();

        [Test]
        public Task ThisTest() => ThisTestImpl();

        [Test]
        public Task UnsafeDoesNotImpactDllImportTest() => UnsafeDoesNotImpactDllImportTestImpl();

        [Test]
        public Task VirtualTest() => VirtualTestImpl();

        [Test]
        public Task VirtualWithVtblIndexAttributeTest() => VirtualWithVtblIndexAttributeTestImpl();

        [Test]
        public virtual Task MacrosExpansionTest()
        {
            var inputContents = @"typedef struct
{
	unsigned char *buf;
	int size;
} context_t;

int buf_close(void *pcontext)
{
	((context_t*)pcontext)->buf=0;
	return 0;
}
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public unsafe partial struct context_t
    {
        [NativeTypeName(""unsigned char *"")]
        public byte* buf;

        public int size;
    }

    public static unsafe partial class Methods
    {
        public static int buf_close(void* pcontext)
        {
            ((context_t*)(pcontext))->buf = null;
            return 0;
        }
    }
}
";

            return ValidateBindingsAsync(inputContents, expectedOutputContents);
        }

        protected abstract Task ConstructorTestImpl();

        protected abstract Task ConstructorWithInitializeTestImpl();

        protected abstract Task ConversionTestImpl();

        protected abstract Task DestructorTestImpl();

        protected abstract Task InstanceTestImpl();

        protected abstract Task MemberCallTestImpl();

        protected abstract Task MemberTestImpl();

        protected abstract Task NewKeywordTestImpl();

        protected abstract Task NewKeywordVirtualTestImpl();

        protected abstract Task NewKeywordVirtualWithExplicitVtblTestImpl();

        protected abstract Task OperatorTestImpl();

        protected abstract Task OperatorCallTestImpl();

        protected abstract Task StaticTestImpl();

        protected abstract Task ThisTestImpl();

        protected abstract Task UnsafeDoesNotImpactDllImportTestImpl();

        protected abstract Task VirtualTestImpl();

        protected abstract Task VirtualWithVtblIndexAttributeTestImpl();

        protected abstract Task ValidateBindingsAsync(string inputContents, string expectedOutputContents);
    }
}
