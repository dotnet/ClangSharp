// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.
// Ported from https://github.com/dotnet/clangsharp/blob/main/sources/libClangSharp

using NUnit.Framework;
using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Interop.UnitTests
{
    /// <summary>Provides validation of the <see cref="CX_TemplateArgument" /> struct.</summary>
    public static unsafe partial class CX_TemplateArgumentTests
    {
        /// <summary>Validates that the <see cref="CX_TemplateArgument" /> struct is blittable.</summary>
        [Test]
        public static void IsBlittableTest()
        {
            Assert.That(Marshal.SizeOf<CX_TemplateArgument>(), Is.EqualTo(sizeof(CX_TemplateArgument)));
        }

        /// <summary>Validates that the <see cref="CX_TemplateArgument" /> struct has the right <see cref="LayoutKind" />.</summary>
        [Test]
        public static void IsLayoutSequentialTest()
        {
            Assert.That(typeof(CX_TemplateArgument).IsLayoutSequential, Is.True);
        }

        /// <summary>Validates that the <see cref="CX_TemplateArgument" /> struct has the correct size.</summary>
        [Test]
        public static void SizeOfTest()
        {
            if (Environment.Is64BitProcess)
            {
                Assert.That(sizeof(CX_TemplateArgument), Is.EqualTo(24));
            }
            else
            {
                Assert.That(sizeof(CX_TemplateArgument), Is.EqualTo(16));
            }
        }
    }
}
