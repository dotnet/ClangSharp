// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-11.0.0/clang/include/clang-c
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

using System;
using System.Runtime.InteropServices;
using Xunit;

namespace ClangSharp.Interop.UnitTests
{
    /// <summary>Provides validation of the <see cref="CXTUResourceUsageEntry" /> struct.</summary>
    public static unsafe class CXTUResourceUsageEntryTests
    {
        /// <summary>Validates that the <see cref="CXTUResourceUsageEntry" /> struct is blittable.</summary>
        [Fact]
        public static void IsBlittableTest()
        {
            Assert.Equal(sizeof(CXTUResourceUsageEntry), Marshal.SizeOf<CXTUResourceUsageEntry>());
        }

        /// <summary>Validates that the <see cref="CXTUResourceUsageEntry" /> struct has the right <see cref="LayoutKind" />.</summary>
        [Fact]
        public static void IsLayoutSequentialTest()
        {
            Assert.True(typeof(CXTUResourceUsageEntry).IsLayoutSequential);
        }

        /// <summary>Validates that the <see cref="CXTUResourceUsageEntry" /> struct has the correct size.</summary>
        [Fact]
        public static void SizeOfTest()
        {
            if (Environment.Is64BitProcess)
            {
                Assert.Equal(16, sizeof(CXTUResourceUsageEntry));
            }
            else
            {
                Assert.Equal(8, sizeof(CXTUResourceUsageEntry));
            }
        }
    }
}
