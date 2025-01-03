// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using static ClangSharp.Interop.CXTranslationUnit_Flags;

namespace ClangSharp.UnitTests;

[Parallelizable(ParallelScope.All)]
public abstract class PInvokeGeneratorTest(PInvokeGeneratorOutputMode outputMode, PInvokeGeneratorConfigurationOptions outputVersion)
{
    private readonly PInvokeGeneratorOutputMode _outputMode = outputMode;
    private readonly string _baselineFileExtension = outputMode == PInvokeGeneratorOutputMode.CSharp ? "cs" : "xml";
    private readonly PInvokeGeneratorConfigurationOptions _outputVersion = outputVersion;
    private readonly string _outputVersionAsString = OutputVersionToString(outputVersion);

    private const string UpdateBaselineEnvVar = "AUTO_UPDATE_BASELINE";

    private const string DefaultInputFileName = "ClangUnsavedFile.h";
    private const string DefaultLibraryPath = "ClangSharpPInvokeGenerator";
    private const string DefaultNamespaceName = "ClangSharp.Test";

    private const CXTranslationUnit_Flags DefaultTranslationUnitFlags = CXTranslationUnit_IncludeAttributedTypes          // Include attributed types in CXType
                                                                        | CXTranslationUnit_VisitImplicitAttributes         // Implicit attributes should be visited
                                                                        | CXTranslationUnit_DetailedPreprocessingRecord;
    private const string DefaultCppStandard = "c++17";

    private static readonly string[] s_defaultCppClangCommandLineArgs =
    [
        $"-std={DefaultCppStandard}",                           // The input files should be compiled for C++ 17
        "-xc++",                                // The input files are C++
        "-Wno-pragma-once-outside-header",      // We are processing files which may be header files
        "-Wno-c++11-narrowing"
    ];

    private static readonly string s_osName = OperatingSystem.IsWindows() ? "Windows" : "Unix";

    private static readonly bool s_autoUpdateBaselines = Environment.GetEnvironmentVariable(UpdateBaselineEnvVar) == "1";
    private static readonly string s_baselineDirectory = FindBaselineDirectory();

    protected static IEnumerable<object[]> FixtureArgs()
    {
        foreach (var outputLang in new[] { PInvokeGeneratorOutputMode.CSharp, PInvokeGeneratorOutputMode.Xml })
        {
            foreach (var outputVersion in new[] {
                PInvokeGeneratorConfigurationOptions.GenerateCompatibleCode,
                PInvokeGeneratorConfigurationOptions.None,
                PInvokeGeneratorConfigurationOptions.GenerateLatestCode,
                PInvokeGeneratorConfigurationOptions.GeneratePreviewCode })
            {
                yield return new object[] { outputLang, outputVersion }; 
            }
        }
    }

    private static string FindBaselineDirectory()
    {
        var currentDirectory = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);
        do 
        {
            if (File.Exists(Path.Join(currentDirectory.FullName, "ClangSharp.sln")))
            {
                return Path.Join(currentDirectory.FullName, "tests", "ClangSharp.PInvokeGenerator.UnitTests", "Baselines");
            }
            currentDirectory = currentDirectory.Parent;
        } while (currentDirectory != null);

        Assert.Fail("Could not find ClangSharp.sln, test artifacts must be in a child directory of the source");
        return string.Empty;
    }

    private static string OutputVersionToString(PInvokeGeneratorConfigurationOptions outputVersion)
    {
        switch (outputVersion)
        {
            case PInvokeGeneratorConfigurationOptions.GenerateCompatibleCode:
                return "Compatible";
            case PInvokeGeneratorConfigurationOptions.None:
                return "Default";
            case PInvokeGeneratorConfigurationOptions.GenerateLatestCode:
                return "Latest";
            case PInvokeGeneratorConfigurationOptions.GeneratePreviewCode:
                return "Preview";
            default:
                Assert.Fail("Unknown output version");
                return string.Empty;
        }
    }

    private static async ValueTask<bool> UpdateBaselineIfRequestedAsync(string path, string contents)
    {
        if (s_autoUpdateBaselines)
        {
            TestContext.WriteLine($"Updating baseline {Path.GetFileName(path)}.");
            await File.WriteAllTextAsync(path, contents).ConfigureAwait(false);
            return true;
        }
        return false;
    }

    protected async Task ValidateGeneratedBindingsAsync(
        string inputContents,
        PInvokeGeneratorConfigurationOptions additionalConfigOptions = PInvokeGeneratorConfigurationOptions.None,
        string[]? excludedNames = null,
        IReadOnlyDictionary<string, string>? remappedNames = null,
        IReadOnlyDictionary<string, AccessSpecifier>? withAccessSpecifiers = null,
        IReadOnlyDictionary<string, IReadOnlyList<string>>? withAttributes = null,
        IReadOnlyDictionary<string, string>? withCallConvs = null,
        IReadOnlyDictionary<string, string>? withClasses = null,
        IReadOnlyDictionary<string, string>? withLibraryPaths = null,
        IReadOnlyDictionary<string, string>? withNamespaces = null,
        string[]? withSetLastErrors = null,
        IReadOnlyDictionary<string, (string, PInvokeGeneratorTransparentStructKind)>? withTransparentStructs = null,
        IReadOnlyDictionary<string, string>? withTypes = null,
        IReadOnlyDictionary<string, IReadOnlyList<string>>? withUsings = null,
        IReadOnlyDictionary<string, string>? withPackings = null,
        IEnumerable<Diagnostic>? expectedDiagnostics = null,
        string libraryPath = DefaultLibraryPath,
        string[]? commandLineArgs = null,
        string language = "c++",
        string languageStandard = DefaultCppStandard,
        Func<string, string>? osxTransform = null)
    {
        Assert.That(DefaultInputFileName, Does.Exist);
        commandLineArgs ??= s_defaultCppClangCommandLineArgs;

        var configOptions = additionalConfigOptions | _outputVersion | PInvokeGeneratorConfigurationOptions.GenerateMacroBindings;

        using var outputStream = new MemoryStream();
        using var unsavedFile = CXUnsavedFile.Create(DefaultInputFileName, inputContents);

        var unsavedFiles = new CXUnsavedFile[] { unsavedFile };
        var config = new PInvokeGeneratorConfiguration(language, languageStandard, DefaultNamespaceName, Path.GetRandomFileName(), headerFile: null, _outputMode, configOptions) {
            DefaultClass = null,
            ExcludedNames = excludedNames,
            IncludedNames = null,
            LibraryPath = libraryPath,
            MethodPrefixToStrip = null,
            RemappedNames = remappedNames,
            TraversalNames = null,
            TestOutputLocation = null,
            WithAccessSpecifiers = withAccessSpecifiers,
            WithAttributes = withAttributes,
            WithCallConvs = withCallConvs,
            WithClasses = withClasses,
            WithLibraryPaths = withLibraryPaths,
            WithManualImports = null,
            WithNamespaces = withNamespaces,
            WithSetLastErrors = withSetLastErrors,
            WithSuppressGCTransitions = null,
            WithTransparentStructs = withTransparentStructs,
            WithTypes = withTypes,
            WithUsings = withUsings,
            WithPackings = withPackings,
        };

        using (var pinvokeGenerator = new PInvokeGenerator(config, (path) => outputStream))
        {
            var handle = CXTranslationUnit.Parse(pinvokeGenerator.IndexHandle, DefaultInputFileName, commandLineArgs, unsavedFiles, DefaultTranslationUnitFlags);

            using var translationUnit = TranslationUnit.GetOrCreate(handle);
            Debug.Assert(translationUnit is not null);

            pinvokeGenerator.GenerateBindings(translationUnit, DefaultInputFileName, commandLineArgs, DefaultTranslationUnitFlags);

            if (expectedDiagnostics is null)
            {
                Assert.That(pinvokeGenerator.Diagnostics, Is.Empty);
            }
            else
            {
                Assert.That(pinvokeGenerator.Diagnostics, Is.EqualTo(expectedDiagnostics));
            }
        }
        outputStream.Position = 0;

        using var streamReader = new StreamReader(outputStream);
        var actualOutputContents = await streamReader.ReadToEndAsync().ConfigureAwait(false);

        if (osxTransform is not null && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            actualOutputContents = osxTransform(actualOutputContents);
        }

        var testClassName = TestContext.CurrentContext.Test.ClassName?.Substring((TestContext.CurrentContext.Test.Namespace?.Length ?? -1) + 1) ?? "";
        var arguments = string.Empty;
        foreach (var arg in TestContext.CurrentContext.Test.Arguments)
        {
            arguments += string.Format(CultureInfo.InvariantCulture, ".{0}", arg);
        }
        arguments = arguments.Replace("<", "__lt", StringComparison.Ordinal)
            .Replace(">", "__gt", StringComparison.Ordinal)
            .Replace(":", "__cln", StringComparison.Ordinal)
            .Replace("\"", "__quot", StringComparison.Ordinal)
            .Replace("/", "__fs", StringComparison.Ordinal)
            .Replace("\\", "__bs", StringComparison.Ordinal)
            .Replace("|", "__vl", StringComparison.Ordinal)
            .Replace("?", "__qm", StringComparison.Ordinal)
            .Replace("*", "__ast", StringComparison.Ordinal);
        var testFilename = $"{testClassName}.{TestContext.CurrentContext.Test.MethodName}{arguments}.{s_osName}.{_outputVersionAsString}.{_baselineFileExtension}";
        var expectedOutputPath = Path.Join(s_baselineDirectory, testFilename);

        if (!File.Exists(expectedOutputPath))
        {
            if (!await UpdateBaselineIfRequestedAsync(expectedOutputPath, actualOutputContents).ConfigureAwait(true))
            {
                Assert.Fail($"Baseline does not exist. Set {UpdateBaselineEnvVar}=1 to automatically generate it.");
            }
        }
        else
        {
            var expectedOutputContents = await File.ReadAllTextAsync(expectedOutputPath).ConfigureAwait(false);
            if (expectedOutputContents != actualOutputContents)
            {
                if (!await UpdateBaselineIfRequestedAsync(expectedOutputPath, actualOutputContents).ConfigureAwait(true))
                {
                    // Write the actual output and attach it to the test: this allows us to gather the actual output
                    // from CI.
                    var actualOutputPath = Path.Join(TestContext.CurrentContext.WorkDirectory, testFilename);
                    await File.WriteAllTextAsync(actualOutputPath, actualOutputContents).ConfigureAwait(false);
                    TestContext.AddTestAttachment(actualOutputPath);

                    Assert.That(actualOutputContents, Is.EqualTo(expectedOutputContents));
                }
            }
        }
    }
}
