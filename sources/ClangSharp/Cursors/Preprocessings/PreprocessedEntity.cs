// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;

namespace ClangSharp;

public class PreprocessedEntity : Cursor
{
    private protected PreprocessedEntity(CXCursor handle, CXCursorKind expectedCursorKind) : base(handle, expectedCursorKind)
    {
    }

    internal static new PreprocessedEntity Create(CXCursor handle)
    {
        PreprocessedEntity result;

        switch (handle.Kind)
        {
            case CXCursor_PreprocessingDirective:
            {
                result = new PreprocessingDirective(handle);
                break;
            }

            case CXCursor_MacroDefinition:
            {
                result = new MacroDefinitionRecord(handle);
                break;
            }

            case CXCursor_MacroExpansion:
            {
                result = new MacroExpansion(handle);
                break;
            }

            case CXCursor_InclusionDirective:
            {
                result = new InclusionDirective(handle);
                break;
            }

            default:
            {
                Debug.WriteLine($"Unhandled preprocessing kind: {handle.KindSpelling}.");
                result = new PreprocessedEntity(handle, handle.Kind);
                break;
            }
        }

        return result;
    }
}
