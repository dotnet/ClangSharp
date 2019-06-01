using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class CXXConversionDecl : CXXMethodDecl
    {
        public CXXConversionDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_ConversionFunction);
        }
    }
}
