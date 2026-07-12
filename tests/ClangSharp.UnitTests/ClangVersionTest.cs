// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Globalization;
using System.Text.RegularExpressions;
using ClangSharp.Interop;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

public sealed class ClangVersionTest
{
    [Test]
    public void MajorMinorMatchesExpected()
    {
        using var versionString = clang.getClangVersion();
        var versionText = versionString.ToString();

        var match = Regex.Match(versionText, @"version (\d+)\.(\d+)");

        Assert.That(match.Success, Is.True, $"Could not parse version from: {versionText}");

        var major = int.Parse(match.Groups[1].ValueSpan, CultureInfo.InvariantCulture);
        var minor = int.Parse(match.Groups[2].ValueSpan, CultureInfo.InvariantCulture);

        Assert.That(major, Is.EqualTo(clang.MajorVersion), $"libclang major version mismatch. Full version string: {versionText}");
        Assert.That(minor, Is.EqualTo(clang.MinorVersion), $"libclang minor version mismatch. Full version string: {versionText}");
    }
}
