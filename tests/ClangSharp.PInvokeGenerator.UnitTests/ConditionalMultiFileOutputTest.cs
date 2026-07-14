// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using ClangSharp.Interop;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

public sealed class ConditionalMultiFileOutputTest : PInvokeGeneratorTest
{
    // dotnet/ClangSharp#365: --with-conditional wraps output in a leading `#if <symbol>` and trailing
    // `#endif`. This covers multi-file mode, where each generated file is wrapped using the same single
    // global symbol. The golden-file harness is single-stream, so this drives the generator directly with a
    // per-path capturing stream factory.
    [Test]
    public void WrapsEachFileInConditional()
    {
        const string InputContents = @"struct StructOne
{
    int x;
};

struct StructTwo
{
    int y;
};
";

        var files = GenerateMultiFileBindings(InputContents, withConditional: "VERSION_2_OR_NEWER");

        Assert.That(files, Has.Count.EqualTo(2));

        foreach (var (path, contents) in files)
        {
            Assert.That(contents, Does.StartWith("#if VERSION_2_OR_NEWER\n"), $"'{path}' should open with the conditional");
            Assert.That(contents.TrimEnd('\n'), Does.EndWith("#endif"), $"'{path}' should close with #endif");
        }
    }

    private static Dictionary<string, string> GenerateMultiFileBindings(string inputContents, string withConditional)
    {
        Assert.That(DefaultInputFileName, Does.Exist);

        var outputDirectory = Directory.CreateTempSubdirectory("ClangSharpConditionalTest");

        // MemoryStream.ToArray() remains valid after disposal, so the captured contents survive the
        // per-file StreamWriter (opened with leaveOpen: false) disposing the underlying stream.
        var streams = new Dictionary<string, MemoryStream>(StringComparer.Ordinal);

        try
        {
            using var unsavedFile = CXUnsavedFile.Create(DefaultInputFileName, inputContents);
            var unsavedFiles = new CXUnsavedFile[] { unsavedFile };

            var config = new PInvokeGeneratorConfiguration("c++", DefaultCppStandard, DefaultNamespaceName, outputDirectory.FullName, headerFile: null, PInvokeGeneratorOutputMode.CSharp, PInvokeGeneratorConfigurationOptions.GenerateMultipleFiles | PInvokeGeneratorConfigurationOptions.GenerateMacroBindings) {
                LibraryPath = DefaultLibraryPath,
                WithConditional = withConditional,
            };

            Stream StreamFactory(string path)
            {
                if (!streams.TryGetValue(path, out var stream))
                {
                    stream = new MemoryStream();
                    streams.Add(path, stream);
                }
                return stream;
            }

            using (var pinvokeGenerator = new PInvokeGenerator(config, StreamFactory))
            {
                var handle = CXTranslationUnit.Parse(pinvokeGenerator.IndexHandle, DefaultInputFileName, DefaultCppClangCommandLineArgs, unsavedFiles, DefaultTranslationUnitFlags);

                using var translationUnit = TranslationUnit.GetOrCreate(handle);
                Debug.Assert(translationUnit is not null);

                pinvokeGenerator.GenerateBindings(translationUnit, DefaultInputFileName, DefaultCppClangCommandLineArgs, DefaultTranslationUnitFlags);
                Assert.That(pinvokeGenerator.Diagnostics, Is.Empty);
            }

            var result = new Dictionary<string, string>(StringComparer.Ordinal);

            foreach (var (path, stream) in streams)
            {
                result.Add(Path.GetFileName(path), Encoding.UTF8.GetString(stream.ToArray()));
            }

            return result;
        }
        finally
        {
            outputDirectory.Delete(recursive: true);
        }
    }
}
