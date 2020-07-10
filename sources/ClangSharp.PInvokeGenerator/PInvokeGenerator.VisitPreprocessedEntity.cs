// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Text;
using ClangSharp.Interop;

namespace ClangSharp
{
    public partial class PInvokeGenerator
    {
        private unsafe void VisitMacroDefinitionRecord(MacroDefinitionRecord macroDefinitionRecord)
        {
            var translationUnitHandle = macroDefinitionRecord.TranslationUnit.Handle;
            var tokens = translationUnitHandle.Tokenize(macroDefinitionRecord.Extent).ToArray();

            if (macroDefinitionRecord.IsFunctionLike)
            {
                AddDiagnostic(DiagnosticLevel.Warning, $"Function like macro definition records are not supported: '{macroDefinitionRecord.Name}'. Generated bindings may be incomplete.", macroDefinitionRecord);
            }
            else if ((tokens[0].Kind == Interop.CXTokenKind.CXToken_Identifier) && (tokens[0].GetSpelling(translationUnitHandle).CString == macroDefinitionRecord.Spelling))
            {
                if (tokens.Length == 1)
                {
                    // Nothing to do for simple macro definitions with no value
                    return;
                }

                var macroName = $"ClangSharpMacro_{macroDefinitionRecord.Spelling}";

                var file = translationUnitHandle.GetFile(_filePath);
                var fileContents = translationUnitHandle.GetFileContents(file, out var size);
#if NETCOREAPP
                var fileContentsBuilder = new StringBuilder(Encoding.UTF8.GetString(fileContents));
#else
                var fileContentsBuilder = new StringBuilder(Encoding.UTF8.GetString(fileContents.ToArray()));
#endif

                fileContentsBuilder.Append('\n');
                fileContentsBuilder.Append($"const auto {macroName} = ");

                var sourceRangeEnd = tokens[tokens.Length - 1].GetExtent(translationUnitHandle).End;
                var sourceRangeStart = tokens[1].GetLocation(translationUnitHandle);

                var sourceRange = CXSourceRange.Create(sourceRangeStart, sourceRangeEnd);

                var macroValue = GetSourceRangeContents(translationUnitHandle, sourceRange);
                fileContentsBuilder.Append(macroValue);

                fileContentsBuilder.Append(';');
                fileContentsBuilder.Append('\n');

                using var unsavedFile = CXUnsavedFile.Create(_filePath, fileContentsBuilder.ToString());
                var unsavedFiles = new CXUnsavedFile[] { unsavedFile };

                var translationFlags = _translationFlags & ~CXTranslationUnit_Flags.CXTranslationUnit_DetailedPreprocessingRecord;
                var handle = CXTranslationUnit.Parse(IndexHandle, _filePath, _clangCommandLineArgs, unsavedFiles, translationFlags);

                using var translationUnit = TranslationUnit.GetOrCreate(handle);

                foreach (var decl in translationUnit.TranslationUnitDecl.Decls)
                {
                    if ((decl is VarDecl varDecl) && (varDecl.Name == macroName))
                    {
                        Visit(varDecl);
                    }
                }
            }
            else
            {
                AddDiagnostic(DiagnosticLevel.Error, $"Unsupported macro definition record: {macroDefinitionRecord.Name}. Generated bindings may be incomplete.", macroDefinitionRecord);
            }
        }

        private void VisitPreprocessingDirective(PreprocessingDirective preprocessingDirective)
        {
            if (preprocessingDirective is InclusionDirective)
            {
            }
            else if (preprocessingDirective is MacroDefinitionRecord macroDefinitionRecord)
            {
                VisitMacroDefinitionRecord(macroDefinitionRecord);
            }
            else
            {
                AddDiagnostic(DiagnosticLevel.Error, $"Unsupported preprocessing directive: '{preprocessingDirective.CursorKind}'. Generated bindings may be incomplete.", preprocessingDirective);
            }
        }

        private void VisitPreprocessedEntity(PreprocessedEntity preprocessedEntity)
        {
            if (!_config.GenerateMacroBindings)
            {
                return;
            }

            if (IsExcluded(preprocessedEntity))
            {
                return;
            }

            if (preprocessedEntity is MacroExpansion)
            {
                // Not currently handling macro expansions
            }
            else if (preprocessedEntity is PreprocessingDirective preprocessingDirective)
            {
                VisitPreprocessingDirective(preprocessingDirective);
            }
            else
            {
                AddDiagnostic(DiagnosticLevel.Error, $"Unsupported preprocessed entity: '{preprocessedEntity.CursorKind}'. Generated bindings may be incomplete.", preprocessedEntity);
            }
        }
    }
}
