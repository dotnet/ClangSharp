// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-13.0.0/clang/include/clang-c
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

using System;
using System.Runtime.InteropServices;
using NUnit.Framework;

namespace ClangSharp.Interop.UnitTests
{
    /// <summary>Provides validation of the <see cref="CXPlatformAvailability" /> struct.</summary>
    public static unsafe class CXPlatformAvailabilityTests
    {
        /// <summary>Validates that the <see cref="CXPlatformAvailability" /> struct is blittable.</summary>
        [Test]
        public static void IsBlittableTest()
        {
            Assert.AreEqual(sizeof(CXPlatformAvailability), Marshal.SizeOf<CXPlatformAvailability>());
        }

        /// <summary>Validates that the <see cref="CXPlatformAvailability" /> struct has the right <see cref="LayoutKind" />.</summary>
        [Test]
        public static void IsLayoutSequentialTest()
        {
            Assert.True(typeof(CXPlatformAvailability).IsLayoutSequential);
        }

        /// <summary>Validates that the <see cref="CXPlatformAvailability" /> struct has the correct size.</summary>
        [Test]
        public static void SizeOfTest()
        {
            if (Environment.Is64BitProcess)
            {
                Assert.AreEqual(72, sizeof(CXPlatformAvailability));
            }
            else
            {
                Assert.AreEqual(56, sizeof(CXPlatformAvailability));
            }
        }
    }
}
