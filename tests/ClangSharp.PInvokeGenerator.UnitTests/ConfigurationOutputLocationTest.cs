// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.IO;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

public sealed class ConfigurationOutputLocationTest
{
    private static PInvokeGeneratorConfiguration CreateConfiguration(string outputLocation, PInvokeGeneratorConfigurationOptions options)
    {
        return new PInvokeGeneratorConfiguration("c++", "c++17", "ClangSharp.Test", outputLocation, headerFile: null, PInvokeGeneratorOutputMode.CSharp, options);
    }

    [Test]
    public void SingleFileModeRejectsExistingDirectory()
    {
        var directory = Directory.CreateTempSubdirectory("ClangSharpTest");

        try
        {
            var exception = Assert.Throws<System.ArgumentException>(() => CreateConfiguration(directory.FullName, PInvokeGeneratorConfigurationOptions.None))!;
            Assert.That(exception.Message, Does.Contain("existing directory"));
            Assert.That(exception.Message, Does.Contain("--config multi-file"));
        }
        finally
        {
            directory.Delete(recursive: true);
        }
    }

    [Test]
    public void MultiFileModeRejectsExistingFile()
    {
        var file = Path.GetTempFileName();

        try
        {
            var exception = Assert.Throws<System.ArgumentException>(() => CreateConfiguration(file, PInvokeGeneratorConfigurationOptions.GenerateMultipleFiles))!;
            Assert.That(exception.Message, Does.Contain("existing file"));
            Assert.That(exception.Message, Does.Contain("--config multi-file"));
        }
        finally
        {
            File.Delete(file);
        }
    }

    [Test]
    public void SingleFileModeAllowsExistingFile()
    {
        var file = Path.GetTempFileName();

        try
        {
            Assert.DoesNotThrow(() => CreateConfiguration(file, PInvokeGeneratorConfigurationOptions.None));
        }
        finally
        {
            File.Delete(file);
        }
    }

    [Test]
    public void MultiFileModeAllowsExistingDirectory()
    {
        var directory = Directory.CreateTempSubdirectory("ClangSharpTest");

        try
        {
            Assert.DoesNotThrow(() => CreateConfiguration(directory.FullName, PInvokeGeneratorConfigurationOptions.GenerateMultipleFiles));
        }
        finally
        {
            directory.Delete(recursive: true);
        }
    }

    [Test]
    public void NonExistentOutputLocationIsAllowedInEitherMode()
    {
        var path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

        Assert.DoesNotThrow(() => CreateConfiguration(path, PInvokeGeneratorConfigurationOptions.None));
        Assert.DoesNotThrow(() => CreateConfiguration(path, PInvokeGeneratorConfigurationOptions.GenerateMultipleFiles));
    }
}
