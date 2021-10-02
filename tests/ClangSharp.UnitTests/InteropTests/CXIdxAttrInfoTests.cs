// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-13.0.0/clang/include/clang-c
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

using System;
using System.Runtime.InteropServices;
using Xunit;

namespace ClangSharp.Interop.UnitTests
{
    /// <summary>Provides validation of the <see cref="CXIdxAttrInfo" /> struct.</summary>
    public static unsafe class CXIdxAttrInfoTests
    {
        /// <summary>Validates that the <see cref="CXIdxAttrInfo" /> struct is blittable.</summary>
        [Fact]
        public static void IsBlittableTest()
        {
            Assert.Equal(sizeof(CXIdxAttrInfo), Marshal.SizeOf<CXIdxAttrInfo>());
        }

        /// <summary>Validates that the <see cref="CXIdxAttrInfo" /> struct has the right <see cref="LayoutKind" />.</summary>
        [Fact]
        public static void IsLayoutSequentialTest()
        {
            Assert.True(typeof(CXIdxAttrInfo).IsLayoutSequential);
        }

        /// <summary>Validates that the <see cref="CXIdxAttrInfo" /> struct has the correct size.</summary>
        [Fact]
        public static void SizeOfTest()
        {
            if (Environment.Is64BitProcess)
            {
                Assert.Equal(64, sizeof(CXIdxAttrInfo));
            }
            else
            {
                Assert.Equal(36, sizeof(CXIdxAttrInfo));
            }
        }
    }
}
