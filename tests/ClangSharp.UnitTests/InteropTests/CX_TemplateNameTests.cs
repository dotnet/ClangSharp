// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.
// Ported from https://github.com/dotnet/clangsharp/blob/main/sources/libClangSharp

using System;
using System.Runtime.InteropServices;
using NUnit.Framework;

namespace ClangSharp.Interop.UnitTests
{
    /// <summary>Provides validation of the <see cref="CX_TemplateName" /> struct.</summary>
    public static unsafe class CX_TemplateNameTests
    {
        /// <summary>Validates that the <see cref="CX_TemplateName" /> struct is blittable.</summary>
        [Test]
        public static void IsBlittableTest()
        {
            Assert.AreEqual(sizeof(CX_TemplateName), Marshal.SizeOf<CX_TemplateName>());
        }

        /// <summary>Validates that the <see cref="CX_TemplateName" /> struct has the right <see cref="LayoutKind" />.</summary>
        [Test]
        public static void IsLayoutSequentialTest()
        {
            Assert.True(typeof(CX_TemplateName).IsLayoutSequential);
        }

        /// <summary>Validates that the <see cref="CX_TemplateName" /> struct has the correct size.</summary>
        [Test]
        public static void SizeOfTest()
        {
            if (Environment.Is64BitProcess)
            {
                Assert.AreEqual(24, sizeof(CX_TemplateName));
            }
            else
            {
                Assert.AreEqual(12, sizeof(CX_TemplateName));
            }
        }
    }
}
