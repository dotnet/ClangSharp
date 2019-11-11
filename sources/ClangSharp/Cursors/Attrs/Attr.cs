// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class Attr : Cursor
    {
        private protected Attr(CXCursor handle, CXCursorKind expectedKind) : base(handle, expectedKind)
        {
        }

        internal static new Attr Create(CXCursor handle)
        {
            Attr result;

            switch (handle.Kind)
            {
                case CXCursorKind.CXCursor_UnexposedAttr:
                {
                    result = new Attr(handle, CXCursorKind.CXCursor_UnexposedAttr);
                    break;
                }

                case CXCursorKind.CXCursor_CXXFinalAttr:
                case CXCursorKind.CXCursor_PureAttr:
                case CXCursorKind.CXCursor_ConstAttr:
                case CXCursorKind.CXCursor_VisibilityAttr:
                case CXCursorKind.CXCursor_DLLExport:
                case CXCursorKind.CXCursor_DLLImport:
                {
                    result = new InheritableAttr(handle, handle.Kind);
                    break;
                }

                default:
                {
                    Debug.WriteLine($"Unhandled attribute kind: {handle.KindSpelling}.");
                    Debugger.Break();

                    result = new Attr(handle, handle.kind);
                    break;
                }
            }

            return result;
        }
    }
}
