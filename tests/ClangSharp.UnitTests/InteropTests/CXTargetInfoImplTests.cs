// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-13.0.0/clang/include/clang-c
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

using System.Runtime.InteropServices;
using Xunit;

namespace ClangSharp.Interop.UnitTests
{
    /// <summary>Provides validation of the <see cref="CXTargetInfoImpl" /> struct.</summary>
    public static unsafe class CXTargetInfoImplTests
    {
        /// <summary>Validates that the <see cref="CXTargetInfoImpl" /> struct is blittable.</summary>
        [Fact]
        public static void IsBlittableTest()
        {
            Assert.Equal(sizeof(CXTargetInfoImpl), Marshal.SizeOf<CXTargetInfoImpl>());
        }

        /// <summary>Validates that the <see cref="CXTargetInfoImpl" /> struct has the right <see cref="LayoutKind" />.</summary>
        [Fact]
        public static void IsLayoutSequentialTest()
        {
            Assert.True(typeof(CXTargetInfoImpl).IsLayoutSequential);
        }

        /// <summary>Validates that the <see cref="CXTargetInfoImpl" /> struct has the correct size.</summary>
        [Fact]
        public static void SizeOfTest()
        {
            Assert.Equal(1, sizeof(CXTargetInfoImpl));
        }
    }
}
