// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-13.0.0/clang/include/clang-c
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

using System;
using System.Runtime.InteropServices;
using Xunit;

namespace ClangSharp.Interop.UnitTests
{
    /// <summary>Provides validation of the <see cref="CXCodeCompleteResults" /> struct.</summary>
    public static unsafe class CXCodeCompleteResultsTests
    {
        /// <summary>Validates that the <see cref="CXCodeCompleteResults" /> struct is blittable.</summary>
        [Fact]
        public static void IsBlittableTest()
        {
            Assert.Equal(sizeof(CXCodeCompleteResults), Marshal.SizeOf<CXCodeCompleteResults>());
        }

        /// <summary>Validates that the <see cref="CXCodeCompleteResults" /> struct has the right <see cref="LayoutKind" />.</summary>
        [Fact]
        public static void IsLayoutSequentialTest()
        {
            Assert.True(typeof(CXCodeCompleteResults).IsLayoutSequential);
        }

        /// <summary>Validates that the <see cref="CXCodeCompleteResults" /> struct has the correct size.</summary>
        [Fact]
        public static void SizeOfTest()
        {
            if (Environment.Is64BitProcess)
            {
                Assert.Equal(16, sizeof(CXCodeCompleteResults));
            }
            else
            {
                Assert.Equal(8, sizeof(CXCodeCompleteResults));
            }
        }
    }
}
