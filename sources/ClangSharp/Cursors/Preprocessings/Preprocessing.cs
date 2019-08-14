using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class Preprocessing : Cursor
    {
        private protected Preprocessing(CXCursor handle, CXCursorKind expectedKind) : base(handle, expectedKind)
        {
        }

        internal static new Preprocessing Create(CXCursor handle)
        {
            Preprocessing result;

            switch (handle.Kind)
            {
                case CXCursorKind.CXCursor_MacroDefinition:
                {
                    result = new MacroDefinition(handle);
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
                    Debugger.Break();

                    result = new Preprocessing(handle, handle.Kind);
                    break;
                }
            }

            return result;
        }
    }
}
