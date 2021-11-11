// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using ClangSharp.Interop;

namespace ClangSharp
{
    public partial class PInvokeGenerator
    {
        private unsafe void VisitMacroDefinitionRecord(MacroDefinitionRecord macroDefinitionRecord)
        {
            if (IsExcluded(macroDefinitionRecord))
            {
                return;
            }

            if (macroDefinitionRecord.IsFunctionLike)
            {
                AddDiagnostic(DiagnosticLevel.Warning, $"Function like macro definition records are not supported: '{macroDefinitionRecord.Name}'. Generated bindings may be incomplete.", macroDefinitionRecord);
                return;
            }

            var translationUnitHandle = macroDefinitionRecord.TranslationUnit.Handle;
            var tokens = translationUnitHandle.Tokenize(macroDefinitionRecord.Extent).ToArray();

            if ((tokens[0].Kind == CXTokenKind.CXToken_Identifier) && (tokens[0].GetSpelling(translationUnitHandle).CString == macroDefinitionRecord.Spelling))
            {
                if (tokens.Length == 1)
                {
                    // Nothing to do for simple macro definitions with no value
                    return;
                }

                var macroName = $"ClangSharpMacro_{macroDefinitionRecord.Spelling}";

                _ = _fileContentsBuilder.Append('\n');
                _ = _fileContentsBuilder.Append($"const auto {macroName} = ");

                var sourceRangeEnd = tokens[^1].GetExtent(translationUnitHandle).End;
                var sourceRangeStart = tokens[1].GetLocation(translationUnitHandle);

                var sourceRange = CXSourceRange.Create(sourceRangeStart, sourceRangeEnd);

                var macroValue = GetSourceRangeContents(translationUnitHandle, sourceRange);
                _ = _fileContentsBuilder.Append(macroValue);

                _ = _fileContentsBuilder.Append(';');
                _ = _fileContentsBuilder.Append('\n');
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
                // Not currently handling inclusion directives
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
