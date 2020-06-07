// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-10.0.0/clang/include/clang-c
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

using System;
using System.Runtime.InteropServices;
using Xunit;

namespace ClangSharp.Interop.UnitTests
{
    /// <summary>Provides validation of the <see cref="CXSourceLocation" /> struct.</summary>
    public static unsafe class CXSourceLocationTests
    {
        /// <summary>Validates that the <see cref="CXSourceLocation" /> struct is blittable.</summary>
        [Fact]
        public static void IsBlittableTest()
        {
            Assert.Equal(sizeof(CXSourceLocation), Marshal.SizeOf<CXSourceLocation>());
        }

        /// <summary>Validates that the <see cref="CXSourceLocation" /> struct has the right <see cref="LayoutKind" />.</summary>
        [Fact]
        public static void IsLayoutSequentialTest()
        {
            Assert.True(typeof(CXSourceLocation).IsLayoutSequential);
        }

        /// <summary>Validates that the <see cref="CXSourceLocation" /> struct has the correct size.</summary>
        [Fact]
        public static void SizeOfTest()
        {
            if (Environment.Is64BitProcess)
            {
                Assert.Equal(24, sizeof(CXSourceLocation));
            }
            else
            {
                Assert.Equal(12, sizeof(CXSourceLocation));
            }
        }
    }
}
