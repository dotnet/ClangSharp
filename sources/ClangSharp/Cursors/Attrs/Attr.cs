using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class Attr : Cursor
    {
        public static new Attr Create(CXCursor handle, Cursor parent)
        {
            switch (handle.Kind)
            {
                case CXCursorKind.CXCursor_UnexposedAttr:
                {
                    return new UnexposedAttr(handle, parent);
                }

                case CXCursorKind.CXCursor_CXXFinalAttr:
                {
                    return new CXXFinalAttr(handle, parent);
                }

                case CXCursorKind.CXCursor_PureAttr:
                {
                    return new PureAttr(handle, parent);
                }

                case CXCursorKind.CXCursor_ConstAttr:
                {
                    return new ConstAttr(handle, parent);
                }

                case CXCursorKind.CXCursor_VisibilityAttr:
                {
                    return new VisibilityAttr(handle, parent);
                }

                case CXCursorKind.CXCursor_DLLExport:
                {
                    return new DLLExport(handle, parent);
                }

                case CXCursorKind.CXCursor_DLLImport:
                {
                    return new DLLImport(handle, parent);
                }

                default:
                {
                    Debug.WriteLine($"Unhandled attribute kind: {handle.KindSpelling}.");
                    Debugger.Break();
                    return new Attr(handle, parent);
                }
            }
        }

        protected Attr(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.IsAttribute);
        }
    }
}
