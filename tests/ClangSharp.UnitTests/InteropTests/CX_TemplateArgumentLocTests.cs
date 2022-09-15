// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.
// Ported from https://github.com/dotnet/clangsharp/blob/main/sources/libClangSharp

using NUnit.Framework;
using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Interop.UnitTests;

/// <summary>Provides validation of the <see cref="CX_TemplateArgumentLoc" /> struct.</summary>
public static unsafe partial class CX_TemplateArgumentLocTests
{
    /// <summary>Validates that the <see cref="CX_TemplateArgumentLoc" /> struct is blittable.</summary>
    [Test]
    public static void IsBlittableTest()
    {
        Assert.That(Marshal.SizeOf<CX_TemplateArgumentLoc>(), Is.EqualTo(sizeof(CX_TemplateArgumentLoc)));
    }

    /// <summary>Validates that the <see cref="CX_TemplateArgumentLoc" /> struct has the right <see cref="LayoutKind" />.</summary>
    [Test]
    public static void IsLayoutSequentialTest()
    {
        Assert.That(typeof(CX_TemplateArgumentLoc).IsLayoutSequential, Is.True);
    }

    /// <summary>Validates that the <see cref="CX_TemplateArgumentLoc" /> struct has the correct size.</summary>
    [Test]
    public static void SizeOfTest()
    {
        if (Environment.Is64BitProcess)
        {
            Assert.That(sizeof(CX_TemplateArgumentLoc), Is.EqualTo(16));
        }
        else
        {
            Assert.That(sizeof(CX_TemplateArgumentLoc), Is.EqualTo(8));
        }
    }
}
