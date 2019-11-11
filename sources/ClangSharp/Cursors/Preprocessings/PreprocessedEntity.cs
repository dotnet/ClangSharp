// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class PreprocessedEntity : Cursor
    {
        private protected PreprocessedEntity(CXCursor handle, CXCursorKind expectedKind) : base(handle, expectedKind)
        {
        }

        internal static new PreprocessedEntity Create(CXCursor handle)
        {
            PreprocessedEntity result;

            switch (handle.Kind)
            {
                case CXCursorKind.CXCursor_MacroDefinition:
                {
                    result = new MacroDefinitionRecord(handle);
                    break;
                }

                case CXCursorKind.CXCursor_MacroExpansion:
                {
                    result = new MacroExpansion(handle);
                    break;
                }

                case CXCursorKind.CXCursor_PreprocessingDirective:
                {
                    result = new PreprocessingDirective(handle);
                    break;
                }

                case CXCursorKind.CXCursor_InclusionDirective:
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
}
