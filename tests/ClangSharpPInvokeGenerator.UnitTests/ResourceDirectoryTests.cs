// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.IO;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

public sealed class ResourceDirectoryTests
{
    [Test]
    public void ExplicitResourceDirectoryIsHonored()
    {
        var resolved = Program.TryResolveResourceDirectory("/some/resource-dir", additionalArgs: [], detect: ShouldNotDetect, out var resourceDirectory, out var warning);

        Assert.That(resolved, Is.True);
        Assert.That(resourceDirectory, Is.EqualTo("/some/resource-dir"));
        Assert.That(warning, Is.Null);
    }

    [Test]
    public void ExplicitResourceDirectoryWinsOverDetection()
    {
        var resolved = Program.TryResolveResourceDirectory("/explicit", additionalArgs: [], detect: static () => "/detected", out var resourceDirectory, out var warning);

        Assert.That(resolved, Is.True);
        Assert.That(resourceDirectory, Is.EqualTo("/explicit"));
        Assert.That(warning, Is.Null);
    }

    [TestCase("-resource-dir")]
    [TestCase("--resource-dir")]
    [TestCase("-resource-dir=/x")]
    [TestCase("--resource-dir=/x")]
    public void AdditionalResourceDirArgIsRespected(string additionalArg)
    {
        var resolved = Program.TryResolveResourceDirectory(explicitResourceDirectory: null, additionalArgs: [additionalArg], detect: ShouldNotDetect, out var resourceDirectory, out var warning);

        Assert.That(resolved, Is.False);
        Assert.That(resourceDirectory, Is.Null);
        Assert.That(warning, Is.Null);
    }

    // A null detector models auto-detection being off: the user passed
    // --no-resource-directory-detection, or the platform (Windows) relies on the MSVC fallback.
    [Test]
    public void NoDetectorDoesNotProbeOrWarn()
    {
        var resolved = Program.TryResolveResourceDirectory(explicitResourceDirectory: null, additionalArgs: [], detect: null, out var resourceDirectory, out var warning);

        Assert.That(resolved, Is.False);
        Assert.That(resourceDirectory, Is.Null);
        Assert.That(warning, Is.Null);
    }

    [Test]
    public void DetectedDirectoryIsUsed()
    {
        var resolved = Program.TryResolveResourceDirectory(explicitResourceDirectory: null, additionalArgs: [], detect: static () => "/detected", out var resourceDirectory, out var warning);

        Assert.That(resolved, Is.True);
        Assert.That(resourceDirectory, Is.EqualTo("/detected"));
        Assert.That(warning, Is.Null);
    }

    [Test]
    public void MissingDirectoryWarnsButDoesNotFail()
    {
        var resolved = Program.TryResolveResourceDirectory(explicitResourceDirectory: null, additionalArgs: [], detect: static () => null, out var resourceDirectory, out var warning);

        Assert.That(resolved, Is.False);
        Assert.That(resourceDirectory, Is.Null);
        Assert.That(warning, Is.Not.Null);
    }

    [Test]
    public void IsValidResourceDirectoryRequiresBuiltinHeader()
    {
        var root = Path.Combine(Path.GetTempPath(), $"clangsharp-res-{Guid.NewGuid():N}");

        try
        {
            Assert.That(Program.IsValidResourceDirectory(root), Is.False);

            _ = Directory.CreateDirectory(Path.Combine(root, "include"));
            Assert.That(Program.IsValidResourceDirectory(root), Is.False);

            File.WriteAllText(Path.Combine(root, "include", "stddef.h"), "");
            Assert.That(Program.IsValidResourceDirectory(root), Is.True);
        }
        finally
        {
            if (Directory.Exists(root))
            {
                Directory.Delete(root, recursive: true);
            }
        }
    }

    [Test]
    public void HasMatchingMajorVersionComparesTheClangMajor()
    {
        var major = ClangSharp.Interop.clang.MajorVersion;

        Assert.That(Program.HasMatchingMajorVersion($"/usr/lib/llvm-{major}/lib/clang/{major}"), Is.True);
        Assert.That(Program.HasMatchingMajorVersion($"/usr/lib/clang/{major}.0.0"), Is.True);
        Assert.That(Program.HasMatchingMajorVersion($"/usr/lib/clang/{major - 1}"), Is.False);
        Assert.That(Program.HasMatchingMajorVersion("/usr/lib/clang/not-a-version"), Is.False);
    }

    private static string? ShouldNotDetect()
    {
        Assert.Fail("Detection should not have run for this configuration.");
        return null;
    }
}
