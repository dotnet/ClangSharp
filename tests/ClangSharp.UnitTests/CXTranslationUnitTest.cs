// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.IO;
using ClangSharp.Interop;
using Xunit;

namespace ClangSharp.UnitTests
{
    // This is not ported from libclangtest but instead created to test Unicode stuff
    public class CXTranslationUnitTest
    {
        [Theory]
        [InlineData("basic")]
        [InlineData("example with spaces")]
        [InlineData("♫")]
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
                using var translationUnit = CXTranslationUnit.Parse(index, file.FullName, Array.Empty<string>(), Array.Empty<CXUnsavedFile>(), CXTranslationUnit_Flags.CXTranslationUnit_None);
                var clangFile = translationUnit.GetFile(file.FullName);
                Assert.Equal(file.FullName, clangFile.Name.CString);
            }
            finally
            {
                Directory.Delete(dir, true);
            }
        }

        [Theory]
        [InlineData("basic")]
        [InlineData("example with spaces")]
        [InlineData("♫")]
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
                var translationUnit = CXTranslationUnit.Parse(index, file.FullName, Array.Empty<string>(), Array.Empty<CXUnsavedFile>(), CXTranslationUnit_Flags.CXTranslationUnit_None);
                var clangFile = translationUnit.GetFile(file.FullName);
                var clangFileName = clangFile.Name;
                var clangFileNameString = clangFileName.CString;

                Assert.Equal(file.FullName, clangFileNameString);
            }
            finally
            {
                Directory.Delete(dir, true);
            }
        }
    }
}
