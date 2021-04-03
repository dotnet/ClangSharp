// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.IO;
using System.Text;
using ClangSharp.Interop;
using Xunit;

namespace ClangSharp.UnitTests
{
    public abstract class TranslationUnitTest
    {
        protected const string DefaultInputFileName = "ClangUnsavedFile.h";

        protected const CXTranslationUnit_Flags DefaultTranslationUnitFlags = CXTranslationUnit_Flags.CXTranslationUnit_IncludeAttributedTypes      // Include attributed types in CXType
                                                                            | CXTranslationUnit_Flags.CXTranslationUnit_VisitImplicitAttributes;    // Implicit attributes should be visited

        protected static readonly string[] DefaultClangCommandLineArgs = new string[]
        {
            "-std=c++17",                           // The input files should be compiled for C++ 17
            "-xc++",                                // The input files are C++
            "-Wno-pragma-once-outside-header"       // We are processing files which may be header files
        };

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

                    if (diagnostic.Severity is CXDiagnosticSeverity.CXDiagnostic_Error or CXDiagnosticSeverity.CXDiagnostic_Fatal)
                    {
                        invalidTranslationUnitHandle = true;
                        _ = errorDiagnostics.Append(' ', 4);
                        _ = errorDiagnostics.AppendLine(diagnostic.Format(CXDiagnosticDisplayOptions.CXDiagnostic_DisplayOption).ToString());
                    }
                }

                Assert.False(invalidTranslationUnitHandle, errorDiagnostics.ToString());
            }

            return TranslationUnit.GetOrCreate(translationUnit);
        }
    }
}
