// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.IO;
using System.Text;
using ClangSharp.Interop;
using NUnit.Framework;
using static ClangSharp.Interop.CXDiagnosticDisplayOptions;
using static ClangSharp.Interop.CXDiagnosticSeverity;
using static ClangSharp.Interop.CXTranslationUnit_Flags;

namespace ClangSharp.UnitTests;

public abstract class TranslationUnitTest
{
    protected const string DefaultInputFileName = "ClangUnsavedFile.h";

    protected const CXTranslationUnit_Flags DefaultTranslationUnitFlags = CXTranslationUnit_IncludeAttributedTypes      // Include attributed types in CXType
                                                                        | CXTranslationUnit_VisitImplicitAttributes;    // Implicit attributes should be visited

    protected static readonly string[] DefaultClangCommandLineArgs =
    [
        "-std=c++17",                           // The input files should be compiled for C++ 17
        "-xc++",                                // The input files are C++
        "-Wno-pragma-once-outside-header"       // We are processing files which may be header files
    ];

    protected static TranslationUnit CreateTranslationUnit(string inputContents)
    {
        Assert.True(File.Exists(DefaultInputFileName));

        using var unsavedFile = CXUnsavedFile.Create(DefaultInputFileName, inputContents);
        var unsavedFiles = new CXUnsavedFile[] { unsavedFile };

        var index = CXIndex.Create();
        var translationUnit = CXTranslationUnit.Parse(index, DefaultInputFileName, DefaultClangCommandLineArgs, unsavedFiles, DefaultTranslationUnitFlags);

        if (translationUnit.NumDiagnostics != 0)
        {
            var errorDiagnostics = new StringBuilder();
            _ = errorDiagnostics.AppendLine($"The provided {nameof(CXTranslationUnit)} has the following diagnostics which prevent its use:");
            var invalidTranslationUnitHandle = false;

            for (uint i = 0; i < translationUnit.NumDiagnostics; ++i)
            {
                using var diagnostic = translationUnit.GetDiagnostic(i);

                if (diagnostic.Severity is CXDiagnostic_Error or CXDiagnostic_Fatal)
                {
                    invalidTranslationUnitHandle = true;
                    _ = errorDiagnostics.Append(' ', 4);
                    _ = errorDiagnostics.AppendLine(diagnostic.Format(CXDiagnostic_DisplayOption).ToString());
                }
            }

            Assert.False(invalidTranslationUnitHandle, errorDiagnostics.ToString());
        }

        return TranslationUnit.GetOrCreate(translationUnit);
    }
}
