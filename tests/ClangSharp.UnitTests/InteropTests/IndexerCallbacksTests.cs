// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-13.0.0/clang/include/clang-c
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

using System;
using System.Runtime.InteropServices;
using Xunit;

namespace ClangSharp.Interop.UnitTests
{
    /// <summary>Provides validation of the <see cref="IndexerCallbacks" /> struct.</summary>
    public static unsafe class IndexerCallbacksTests
    {
        /// <summary>Validates that the <see cref="IndexerCallbacks" /> struct is blittable.</summary>
        [Fact]
        public static void IsBlittableTest()
        {
            Assert.Equal(sizeof(IndexerCallbacks), Marshal.SizeOf<IndexerCallbacks>());
        }

        /// <summary>Validates that the <see cref="IndexerCallbacks" /> struct has the right <see cref="LayoutKind" />.</summary>
        [Fact]
        public static void IsLayoutSequentialTest()
        {
            Assert.True(typeof(IndexerCallbacks).IsLayoutSequential);
        }

        /// <summary>Validates that the <see cref="IndexerCallbacks" /> struct has the correct size.</summary>
        [Fact]
        public static void SizeOfTest()
        {
            if (Environment.Is64BitProcess)
            {
                Assert.Equal(64, sizeof(IndexerCallbacks));
            }
            else
            {
                Assert.Equal(32, sizeof(IndexerCallbacks));
            }
        }
    }
}
