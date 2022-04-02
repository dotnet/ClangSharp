// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-14.0.0/clang/include/clang-c
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

using System;
using System.Runtime.InteropServices;
using NUnit.Framework;

namespace ClangSharp.Interop.UnitTests
{
    /// <summary>Provides validation of the <see cref="CXCodeCompleteResults" /> struct.</summary>
    public static unsafe class CXCodeCompleteResultsTests
    {
        /// <summary>Validates that the <see cref="CXCodeCompleteResults" /> struct is blittable.</summary>
        [Test]
        public static void IsBlittableTest()
        {
            Assert.AreEqual(sizeof(CXCodeCompleteResults), Marshal.SizeOf<CXCodeCompleteResults>());
        }

        /// <summary>Validates that the <see cref="CXCodeCompleteResults" /> struct has the right <see cref="LayoutKind" />.</summary>
        [Test]
        public static void IsLayoutSequentialTest()
        {
            Assert.True(typeof(CXCodeCompleteResults).IsLayoutSequential);
        }

        /// <summary>Validates that the <see cref="CXCodeCompleteResults" /> struct has the correct size.</summary>
        [Test]
        public static void SizeOfTest()
        {
            if (Environment.Is64BitProcess)
            {
                Assert.AreEqual(16, sizeof(CXCodeCompleteResults));
            }
            else
            {
                Assert.AreEqual(8, sizeof(CXCodeCompleteResults));
            }
        }
    }
}
