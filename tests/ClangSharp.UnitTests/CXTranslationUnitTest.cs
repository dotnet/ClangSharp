// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.IO;
using ClangSharp.Interop;
using NUnit.Framework;
using static ClangSharp.Interop.CXTranslationUnit_Flags;

namespace ClangSharp.UnitTests;

// This is not ported from libclangtest but instead created to test Unicode stuff
public class CXTranslationUnitTest
{
    [TestCase("basic")]
    [TestCase("example with spaces")]
    [TestCase("♫")]
    public void Basic(string name)
    {
        // Create a unique directory
        var dir = Path.GetRandomFileName();
        _ = Directory.CreateDirectory(dir);

        try
        {
            // Create a file with the right name
            var file = new FileInfo(Path.Combine(dir, name + ".c"));
            File.WriteAllText(file.FullName, "int main() { return 0; }");

            using var index = CXIndex.Create();
            using var translationUnit = CXTranslationUnit.Parse(index, file.FullName, Array.Empty<string>(), Array.Empty<CXUnsavedFile>(), CXTranslationUnit_None);
            var clangFile = translationUnit.GetFile(file.FullName);
            Assert.AreEqual(file.FullName, clangFile.Name.CString);
        }
        finally
        {
            Directory.Delete(dir, true);
        }
    }

    [TestCase("basic")]
    [TestCase("example with spaces")]
    [TestCase("♫")]
    public void BasicWrapper(string name)
    {
        // Create a unique directory
        var dir = Path.GetRandomFileName();
        _ = Directory.CreateDirectory(dir);

        try
        {
            // Create a file with the right name
            var file = new FileInfo(Path.Combine(dir, name + ".c"));
            File.WriteAllText(file.FullName, "int main() { return 0; }");

            var index = CXIndex.Create();
            var translationUnit = CXTranslationUnit.Parse(index, file.FullName, Array.Empty<string>(), Array.Empty<CXUnsavedFile>(), CXTranslationUnit_None);
            var clangFile = translationUnit.GetFile(file.FullName);
            var clangFileName = clangFile.Name;
            var clangFileNameString = clangFileName.CString;

            Assert.AreEqual(file.FullName, clangFileNameString);
        }
        finally
        {
            Directory.Delete(dir, true);
        }
    }
}
